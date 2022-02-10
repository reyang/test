using System;
using System.Linq.Expressions;
using System.Reflection;
using OpenTelemetry;

internal class ReentrantExportProcessor<T> : BaseExportProcessor<T>
    where T : class
{
    static ReentrantExportProcessor()
    {
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var ctor = typeof(Batch<T>).GetConstructor(flags, null, new Type[] { typeof(T) }, null);
        var value = Expression.Parameter(typeof(T), null);
        var lambda = Expression.Lambda<Func<T, Batch<T>>>(Expression.New(ctor, value), value);
        CreateBatch = lambda.Compile();
    }

    public ReentrantExportProcessor(BaseExporter<T> exporter)
        : base(exporter)
    {
    }

    protected override void OnExport(T data)
    {
        this.exporter.Export(CreateBatch(data));
    }

    static readonly Func<T, Batch<T>> CreateBatch;
}
