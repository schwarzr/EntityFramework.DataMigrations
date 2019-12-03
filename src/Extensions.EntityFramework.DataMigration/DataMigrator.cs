using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Extensions.EntityFramework.DataMigration
{
    public class DataMigrator
    {
        private static ConcurrentDictionary<Type, Func<DbContext, object, CancellationToken, Task>> _invokerCache = new ConcurrentDictionary<Type, Func<DbContext, object, CancellationToken, Task>>();

        private readonly DbContext _context;
        private readonly Func<DbContext, object, CancellationToken, Task> _invoker;
        private readonly ReadOnlyDictionary<string, Type> _localMigrations;
        private readonly DataMigrationOptions _options;

        public DataMigrator(DbContext context, DataMigrationOptions options)
        {
            _context = context;
            _options = options;

            var contextType = _context.GetType();
            var migrationsType = typeof(IDataMigration<>).MakeGenericType(contextType);

            MigrationsAssembly = string.IsNullOrEmpty(options.MigrationAssembly) ? contextType.Assembly : Assembly.Load(options.MigrationAssembly);
            _localMigrations = new ReadOnlyDictionary<string, Type>(
                                        MigrationsAssembly.GetTypes().Where(p => p.GetInterfaces().Any(x => migrationsType.IsAssignableFrom(x)))
                                            .ToDictionary(GetMigrationId, p => p));

            _invoker = _invokerCache.GetOrAdd(contextType, CreateInoker);
        }

        public Assembly MigrationsAssembly { get; }

        public async Task<IEnumerable<string>> GetAppliedMigrationsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var historyContext = new DataMigrationContext(_context.Database.Connection))
            {
                if (_context.Database.CurrentTransaction != null)
                {
                    historyContext.Database.UseTransaction(_context.Database.CurrentTransaction.UnderlyingTransaction);
                }

                var appliedMigrations = await historyContext.HistoryRows
                                                       .OrderBy(p => p.MigrationId)
                                                       .Select(p => p.MigrationId)
                                                       .ToListAsync(cancellationToken);

                var localMigrationKeys = _localMigrations.Keys.OrderBy(p => p).ToList();

                appliedMigrations = appliedMigrations.Where(p => _localMigrations.ContainsKey(p)).ToList();

                return appliedMigrations;
            }
        }

        public async Task<IEnumerable<string>> GetPendingMigrationsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var appliedMigrations = await GetAppliedMigrationsAsync(cancellationToken);
            appliedMigrations = appliedMigrations.OrderByDescending(p => p);

            var localMigrationKeys = _localMigrations.Keys.OrderBy(p => p).ToList();

            int startIndex = -1;

            if (appliedMigrations.Any())
            {
                startIndex = localMigrationKeys.IndexOf(appliedMigrations.Last());
            }

            return localMigrationKeys.Skip(startIndex + 1);
        }

        public async Task<IEnumerable<string>> MigrateAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var pending = await GetPendingMigrationsAsync(cancellationToken);

            if (pending.Any())
            {
                using (var transaction = _context.Database.BeginTransactionIfNotYetRunnig())
                {
                    using (var historyContext = new DataMigrationContext(_context.Database.Connection))
                    {
                        historyContext.Database.UseTransaction(_context.Database.CurrentTransaction.UnderlyingTransaction);
                        foreach (var item in pending)
                        {
                            var migration = Activator.CreateInstance(_localMigrations[item]);
                            _context.ChangeTracker.Entries().ToList().ForEach(p => p.State = EntityState.Detached);

                            await _invoker(_context, migration, cancellationToken);
                            await _context.SaveChangesAsync();

                            historyContext.HistoryRows.Add(new DataMigrationHistoryRow { MigrationId = item });
                            await historyContext.SaveChangesAsync();
                        }

                        transaction.Commit();
                    }
                }
            }

            return pending;
        }

        private static Func<DbContext, object, CancellationToken, Task> CreateInoker(Type contextType)
        {
            var param = Expression.Parameter(typeof(DbContext), "ctx");
            var param2 = Expression.Parameter(typeof(object), "migration");
            var param3 = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

            var castedMigration = Expression.Convert(param2, typeof(IDataMigration<>).MakeGenericType(contextType));
            var castedContext = Expression.Convert(param, contextType);

            var body = Expression.Call(castedMigration, castedMigration.Type.GetMethod(nameof(IDataMigration<DbContext>.ApplyAsync)), castedContext, param3);

            return Expression.Lambda<Func<DbContext, object, CancellationToken, Task>>(body, param, param2, param3).Compile();
        }

        private static string GetMigrationId(Type migration)
        {
            return migration.GetCustomAttribute<MigrationIdAttribute>()?.MigrationId ?? migration.Name;
        }
    }
}