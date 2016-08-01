using BLL.Entities.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace SCMS.ViewModels
{
    class MaterialsStockResultSet
    {
        public List<WarehouseMaterials> GetResult(string search, string sortOrder, int start, int length, List<WarehouseMaterials> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public int Count(string search, List<WarehouseMaterials> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }

        private IQueryable<WarehouseMaterials> FilterResult(string search, List<WarehouseMaterials> dtResult, List<string> columnFilters)
        {
            IQueryable<WarehouseMaterials> results = dtResult.AsQueryable();

            results = results.Where(p => (search == null || (p.EntryTitle != null && p.EntryTitle.ToLower().Contains(search.ToLower()) || p.EntryType != null && p.EntryType.ToString().ToLower().Contains(search.ToLower())
                || p.Warehouse.WarehouseName != null && p.Warehouse.WarehouseName.ToLower().Contains(search.ToLower()) || p.RawMaterial.MaterialName != null && p.RawMaterial.MaterialName.ToLower().Contains(search.ToLower()) || p.Purpose != null && p.Purpose.ToLower().Contains(search.ToLower()) || p.Quantity != null && p.Quantity.ToString().ToLower().Contains(search.ToLower())
                || p.PostingTime != null && p.PostingTime.ToString().ToLower().Contains(search.ToLower())))
                && (columnFilters[0] == null || (p.EntryTitle != null && p.EntryTitle.ToLower().Contains(columnFilters[0].ToLower())))
                && (columnFilters[1] == null || (p.EntryType != null && p.EntryType.ToString().ToLower().Contains(columnFilters[1].ToLower())))
                && (columnFilters[2] == null || (p.Warehouse.WarehouseName != null && p.Warehouse.WarehouseName.ToLower().Contains(columnFilters[2].ToLower())))
                && (columnFilters[3] == null || (p.RawMaterial.MaterialName != null && p.RawMaterial.MaterialName.ToLower().Contains(columnFilters[3].ToLower())))
                && (columnFilters[4] == null || (p.Purpose != null && p.Purpose.ToLower().Contains(columnFilters[4].ToLower())))
                && (columnFilters[5] == null || (p.Quantity != null && p.Quantity.ToString().ToLower().Contains(columnFilters[5].ToLower())))
                && (columnFilters[6] == null || (p.PostingTime != null && p.PostingTime.ToString().ToLower().Contains(columnFilters[6].ToLower())))
                );

            return results;
        }
    }
}
