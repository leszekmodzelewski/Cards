

namespace GeoLib
{
    using ZwSoft.ZwCAD.ApplicationServices;
    using ZwSoft.ZwCAD.Runtime;
    using System;

    public class AppInitializer : IExtensionApplication
    {
        private void DocumentManager_DocumentCreated(object sender, DocumentCollectionEventArgs e)
        {
        }

        private void DocumentManager_DocumentToBeDestroyed(object sender, DocumentCollectionEventArgs e)
        {
        }

        public void Initialize()
        {
            ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.DocumentCreated += new DocumentCollectionEventHandler(this.DocumentManager_DocumentCreated);
            ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.DocumentToBeDestroyed += new DocumentCollectionEventHandler(this.DocumentManager_DocumentToBeDestroyed);
        }

        public void Terminate()
        {
            ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.DocumentCreated -= new DocumentCollectionEventHandler(this.DocumentManager_DocumentCreated);
            ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.DocumentToBeDestroyed -= new DocumentCollectionEventHandler(this.DocumentManager_DocumentToBeDestroyed);
        }
    }
}

