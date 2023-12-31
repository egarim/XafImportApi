﻿using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.XtraRichEdit.Export.OpenDocument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XafImportApi.Module.BusinessObjects;
using XafImportApi.Module.Import;

namespace XafImportApi.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class ImportController : ViewController
    {
        SimpleAction ImportData;
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public ImportController()
        {
            InitializeComponent();
            ImportData = new SimpleAction(this, "Import", "View");
            ImportData.Execute += ImportData_Execute;

            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        private void ImportData_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ImportService importService = new ImportService();
            RowDef rowDef = new RowDef();
            rowDef.ObjectType = typeof(MainObject).FullName;
            rowDef.Properties.Add(0, new PropertyInfo() { Name = "Name", PropertyType = typeof(string).FullName, PropertyKind = PropertyKind.Primitive });
            rowDef.Properties.Add(1, new PropertyInfo() { Name = "Date", PropertyType = typeof(DateTime).FullName, PropertyKind = PropertyKind.Primitive });
            rowDef.Properties.Add(2, new PropertyInfo() { Name = "Active", PropertyType = typeof(bool).FullName, PropertyKind = PropertyKind.Primitive });
            rowDef.Properties.Add(3, new PropertyInfo() { Name = "RefProp1", PropertyType = typeof(RefObject1).FullName, PropertyKind = PropertyKind.Reference, ReferecePropertyLookup = "Code" });
            rowDef.Properties.Add(4, new PropertyInfo() { Name = "RefProp2", PropertyType = typeof(RefObject2).FullName, PropertyKind = PropertyKind.Reference, ReferecePropertyLookup = "Code" });
            rowDef.Properties.Add(5, new PropertyInfo() { Name = "RefProp3", PropertyType = typeof(RefObject3).FullName, PropertyKind = PropertyKind.Reference, ReferecePropertyLookup = "Code" });
            rowDef.Properties.Add(6, new PropertyInfo() { Name = "RefProp4", PropertyType = typeof(RefObject4).FullName, PropertyKind = PropertyKind.Reference, ReferecePropertyLookup = "Code" });
            rowDef.Properties.Add(7, new PropertyInfo() { Name = "RefProp5", PropertyType = typeof(RefObject5).FullName, PropertyKind = PropertyKind.Reference, ReferecePropertyLookup = "Code" });

            Random rnd = new Random();
            for (int i = 0; i < 1500; i++)
            {
                List<object> row = new List<object>();
                for (int j = 0; j < 8; j++)
                {
                    if (j == 0)
                        row.Add("Name" + i);
                    if (j == 1)
                        row.Add(DateTime.Now);
                    if (j == 2)
                        row.Add(true);
                    if (j == 3)
                        row.Add(rnd.Next(0, 20000).ToString());
                    if (j == 4)
                        row.Add(rnd.Next(0, 20000).ToString());
                    if (j == 5)
                        row.Add(rnd.Next(0, 20000).ToString());
                    if (j == 6)
                        row.Add(rnd.Next(0, 20000).ToString());
                    if (j == 7)
                        row.Add(rnd.Next(0, 20000).ToString());
                   
                    
                }
                rowDef.Rows.Add(row);
            }

            importService.Import(this.Application.CreateObjectSpace(typeof(MainObject)), rowDef);
            // Execute your business logic (https://docs.devexpress.com/eXpressAppFramework/112737/).
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
