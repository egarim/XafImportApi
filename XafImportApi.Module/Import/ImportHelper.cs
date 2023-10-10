using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Pdf.Native.BouncyCastle.Utilities;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DevExpress.Drawing.Printing.Internal.DXPageSizeInfo;

namespace XafImportApi.Module.Import
{
    public enum PropertyKind
    {
        Primitive, Reference, Enum,

    }
    public class PropertyInfo
    {
        public string Name { get; set; }
        public string PropertyType { get; set; }
        public PropertyKind PropertyKind { get; set; }
    }
    public class RowDef
    {
        public string ObjectType { get; set; }
        public Dictionary<int, PropertyInfo> Properties { get; set; }
        public List<List<object>> Rows { get; set; }
    }


    public class ImportHelper
    {
        void Import(IObjectSpace objectSpace, RowDef rowDef)
        {
            List<KeyValuePair<int, PropertyInfo>> RefProperties = rowDef.Properties.Where(p => p.Value.PropertyKind == PropertyKind.Reference).ToList();
            var XpOs = objectSpace as XPObjectSpace;
            ITypesInfo TypesInfo = XpOs.TypesInfo;
            var PageSize = 1000;
            var Pages = GetPageSize(rowDef, PageSize);
            Dictionary<PropertyInfo, XPCollection> Collections = new Dictionary<PropertyInfo, XPCollection>();
            foreach (KeyValuePair<int, PropertyInfo> RefProp in RefProperties)
            {
                Collections.Add(RefProp.Value, BuildCollection(RefProp, rowDef, XpOs.Session, TypesInfo, PageSize, Pages));
            }
            XpOs.Session.BulkLoad(Collections.Select(c => c.Value).ToArray());
            foreach (List<object> Row in rowDef.Rows)
            {
                var Instance=objectSpace.CreateObject(TypesInfo.FindTypeInfo(rowDef.ObjectType).Type) as XPCustomObject;
                for (int i = 0; i < Row.Count; i++)
                {
                    if(rowDef.Properties[i].PropertyKind==PropertyKind.Primitive)
                        Instance.SetMemberValue(rowDef.Properties[i].Name, Row[i]);
                    else
                    {
                        Collections[rowDef.Properties[i]].Filter= new BinaryOperator(rowDef.Properties[i].Name, Row[i]);
                        Instance.SetMemberValue(rowDef.Properties[i].Name, Collections[rowDef.Properties[i]][0]);
                    }

                }
            }
            objectSpace.CommitChanges();

        }
        XPCollection BuildCollection(KeyValuePair<int, PropertyInfo> RefProperties, RowDef rowDef, Session session, ITypesInfo TypesInfo, int pageSize, int pages)
        {  
            var Criteria=BuildCriteria(RefProperties, rowDef, pageSize, pages);
            return new XPCollection(PersistentCriteriaEvaluationBehavior.BeforeTransaction, session, TypesInfo.FindTypeInfo(RefProperties.Value.PropertyType).Type, Criteria);
        }
        CriteriaOperator BuildCriteria(KeyValuePair<int, PropertyInfo> RefProperties, RowDef rowDef, int pageSize, int pages)
        {

            List<CriteriaOperator> operators = new List<CriteriaOperator>();
            for (int i = 0; i < pages; i++)
            {
                operators.Add(new InOperator(RefProperties.Value.Name, GetValues(rowDef, i, pageSize, RefProperties.Key)));
            }
            return CriteriaOperator.Or(operators);
           
        }

        int GetPageSize(RowDef rowDef, int pageSize)
        {
            return (int)Math.Ceiling((double)rowDef.Rows.Count() / pageSize);
        }
        List<Object> GetValues(RowDef rowDef, int page, int pageSize, int ColumIndex)
        {
            return rowDef.Rows
                .Select(innerList => innerList[ColumIndex])
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
    }
}

