using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QUANLYTIEC.Models.BUS;
using QUANLYTIEC.Models;

namespace QUANLYTIEC.Controllers
{
    public class TitleEnum
    {
        private static readonly Dictionary<string, int> KeyEntity = new Dictionary<string, int>
        {
            { "Danh sách tài khoản", 1 },
            { "Tạo tài khoản", 2},
            { "Sửa tài khoản", 3},

            { "Danh sách dịch vụ", 4 },
            { "Tạo dịch vụ", 5 },
            { "Sửa dịch vụ", 6 },

            { "Danh sách nhóm món ăn ", 7 },
            { "Tạo nhóm món ăn", 8 },
            { "Sửa nhóm món ăn", 9 },

             { "Danh sách món ăn ", 10 },
            { "Tạo món ăn", 11 },
            { "Sửa món ăn", 12 },

             { "Danh sách nguyên liệu ", 13 },
            { "Tạo nguyên liệu", 14 },
            { "Sửa nguyên liệu", 15 },

            { "Danh sách đơn vị tính ", 16 },
            { "Tạo đơn vị tính", 17 },
            { "Sửa đơn vị tính", 18 },

            { "Danh sách phòng ban ", 19 },
            { "Tạo phòng ban", 20 },
            { "Sửa phòng ban", 21 },

            { "Danh sách nhân viên ", 22 },
            { "Tạo nhân viên", 23 },
            { "Sửa nhân viên", 24 },

            { "Danh sách nhà cung cấp ", 25 },
            { "Tạo nhà cung cấp", 26 },
            { "Sửa nhà cung cấp", 27 },

             { "Danh sách đơn giá lương ", 28 },
            { "Tạo đơn giá lương", 29 },
            { "Sửa đơn giá lương", 30 }
        };
        private static readonly Dictionary<string, int> IndexStartEntity = new Dictionary<string, int>
        {
            {typeof(SYS_USER).Name, 1},
            {typeof(TBL_SERVICE).Name, 4},
            {typeof(TBL_PRODUCT_GROUP).Name, 7},
            {typeof(TBL_PRODUCT).Name, 10},
            {typeof(TBL_PRODUCT_MATERIAL).Name, 13},
            {typeof(TBL_UOM).Name, 16},
            {typeof(TBL_DEPARTMENT).Name, 19},
            {typeof(TBL_EMPLOYEE).Name, 22},
            {typeof(TBL_VENDOR).Name, 25},
            {typeof(TBL_PRICE_PAYROLL).Name, 28},
        };

        /// <summary>
        /// Get title for page 
        /// </summary>
        /// <param name="objectName">name object</param>
        /// <param name="stylePage">style page</param>
        /// <returns></returns>
        public static string getTitleForPage(string objectName, string stylePage)
        {
            int indexStartEntity = IndexStartEntity[objectName];
            indexStartEntity = stylePage == "edit" ? indexStartEntity +=2 : (stylePage == "create" ? ++indexStartEntity : indexStartEntity);
            return KeyEntity.Keys.ElementAt(indexStartEntity - 1);
        }
    }
}