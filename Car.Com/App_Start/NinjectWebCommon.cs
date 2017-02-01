using Car.Com;
using Car.Com.Common.Cache;
using Car.Com.Common.Environment;
using Car.Com.Domain.Services;
using Car.Com.Service;
using Car.Com.Service.Common;
using Car.Com.Service.Data.Impl;
using Car.Com.Service.Rest.Impl;
using Car.Com.Service.Soap.Impl;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Ninject;
using Ninject.Web.Common;
using System;
using System.Web;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(NinjectWebCommon), "Stop")]

namespace Car.Com
{
  public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper Bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            Bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            Bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
          #region Wire-up global low-level services

          kernel.Bind<ILocalCache>()
            .To<LocalCache>()
            .InSingletonScope();

          kernel.Bind<IDbConnectionFactory>()
            .To<SqlConnectionFactory>()
            .InThreadScope();

          #endregion

          #region Wire-up all pre-caching

          kernel.Bind<ICacheable>()
            .To<UriTokenTranslators>()
            .InSingletonScope();

          kernel.Bind<ICacheable>()
            .To<AppEnvironment>()
            .InThreadScope();

          kernel.Bind<ICacheable>()
            .To<MetadataService>()
            .InThreadScope();

          kernel.Bind<ICacheable>()
            .To<SitemapService>()
            .InSingletonScope();

          kernel.Bind<ICacheable>()
            .To<AffiliateService>()
            .InThreadScope();

          kernel.Bind<ICacheable>()
            .To<AssetService>()
            .InSingletonScope();

          kernel.Bind<ICacheable>()
            .To<CarsForSaleService>()
            .InSingletonScope();

          kernel.Bind<ICacheable>()
            .To<EvaluationService>()
            .InSingletonScope();

          #endregion

          #region Return Singletons to consumer

          kernel.Bind<IAssetService>()
            .To<AssetService>()
            .InSingletonScope();

          kernel.Bind<ISitemapService>()
            .To<SitemapService>()
            .InSingletonScope();

          #endregion

          #region Return Instances to consumer

          kernel.Bind<IMetadataService>()
            .To<MetadataService>()
            .InRequestScope();

          kernel.Bind<IAffiliateService>()
            .To<AffiliateService>()
            .InRequestScope();

          kernel.Bind<IVehicleContentService>()
            .To<VehicleContentService>()
            .InRequestScope();

          kernel.Bind<ICarsForSaleService>()
            .To<CarsForSaleService>()
            .InRequestScope();

          kernel.Bind<IVehicleSpecService>()
            .To<VehicleSpecService>()
            .InRequestScope();

          kernel.Bind<IGeoService>()
            .To<GeoService>()
            .InRequestScope();

          kernel.Bind<IImageMetaService>()
            .To<ImageMetaService>()
            .InRequestScope();

          kernel.Bind<IDealerService>()
            .To<DealerService>()
            .InRequestScope();

          kernel.Bind<IEvaluationService>()
            .To<EvaluationService>()
            .InRequestScope();

          kernel.Bind<ILeadService>()
            .To<LeadService>()
            .InRequestScope();

          #endregion

          
          ServiceLocator.Init(kernel);
        }        
    }
}
