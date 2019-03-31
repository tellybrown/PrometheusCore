
namespace PrometheusCore
{
    public interface ICollectorsBuilder
    {
        void Build();
        ICollectorsBuilder RegisterCounter(string name, string help, params string[] labelNames);
        ICollectorsBuilder RegisterGauge(string name, string help, params string[] labelNames);
        ICollectorsBuilder RegisterHistogram(string name, string help, double[] buckets, params string[] labelNames);

        /// <summary>
        /// Registers an interface/class using the attributes registration process
        /// </summary>
        /// <typeparam name="I">Interface of the class to register</typeparam>
        /// <typeparam name="T">Class to register</typeparam>
        /// <param name="background">True means the class will not be utlized by the application but will be handled in the background</param>
        /// <returns></returns>
        ICollectorsBuilder Register<I, T>(bool background = false)
                where T : class, I
                where I : class;
    }
}