using System;
using Microsoft.Extensions.DependencyInjection;
using Niva.Erp.Conta.Reports;

namespace Niva.Erp.Web.Host.Reports.Common
{
    public interface IScopedTestReportDataSourceProvider<T> where T : ReportsAppService
    {
        TestReportDataSourceScope<T> GetTestReportDataSourceScope();
    }

    // NOTE: This provider isolates the rest of the code from the IServiceProvider.
    // That way, we can clearly understand that the consumers of IScopedTestReportDataSourceProvider requre specific scopes...
    public class ScopedTestReportDataSourceProvider<T> : IScopedTestReportDataSourceProvider<T> where T : ReportsAppService
    {
        readonly IServiceProvider provider;

        public ScopedTestReportDataSourceProvider(IServiceProvider provider) {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public TestReportDataSourceScope<T> GetTestReportDataSourceScope() {
            var scope = provider.CreateScope();
            return new TestReportDataSourceScope<T>(scope);
        }
    }

    public class TestReportDataSourceScope<T> : IDisposable where T : ReportsAppService
    {
        readonly IServiceScope scope;
        public T TestReportDataSource { get; private set; }

        public TestReportDataSourceScope(IServiceScope scope) {
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            TestReportDataSource = scope.ServiceProvider.GetRequiredService<T>();
        }

        public void Dispose() {
            scope.Dispose();
        }
    }
}
