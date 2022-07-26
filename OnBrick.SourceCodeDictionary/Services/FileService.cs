using OfficeOpenXml;
using OnBrick.SourceCodeDictionary.Library;
using OnBrick.SourceCodeDictionary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace OnBrick.SourceCodeDictionary.Services
{
    internal class FileService
    {
        const string _excelFileName = "OnBrick.SourceCodeDictionary.xlsx";

        public async Task SaveExcelFile(StorageFolder targetFolder, List<GridItemModel> items)
        {
            string[] header =
            {
                "Seq", "File", "Namespace","FQDN", "MemberType",
                "Public", "Protected", "Private", "Internal", "Partial",
                "Static", "New", "Abstract", "Override", "Type",
                "ReturnType", "Identifier",
            };

            byte[] b;
            using (var p = new ExcelPackage())
            {
                var worksheet = p.Workbook.Worksheets.Add("SourceCode Dictionary");
                int rowIndex = 1;

                // header
                for (int i = 0; i < header.Length; i++)
                {
                    worksheet.Cells[rowIndex, i + 1].Value = header[i];
                }
                rowIndex++;

                // data
                foreach (var item in items)
                {
                    worksheet.Cells[rowIndex, 1].Value = item.Sequence;
                    worksheet.Cells[rowIndex, 2].Value = item.File;
                    worksheet.Cells[rowIndex, 3].Value = item.Namespace;
                    worksheet.Cells[rowIndex, 4].Value = item.FQDN;
                    worksheet.Cells[rowIndex, 5].Value = item.MemberType;

                    worksheet.Cells[rowIndex, 6].Value = item.Public;
                    worksheet.Cells[rowIndex, 7].Value = item.Protected;
                    worksheet.Cells[rowIndex, 8].Value = item.Private;
                    worksheet.Cells[rowIndex, 9].Value = item.Internal;
                    worksheet.Cells[rowIndex, 10].Value = item.Partial;
                    worksheet.Cells[rowIndex, 11].Value = item.Static;
                    worksheet.Cells[rowIndex, 12].Value = item.New;
                    worksheet.Cells[rowIndex, 13].Value = item.Abstract;
                    worksheet.Cells[rowIndex, 14].Value = item.Override;

                    worksheet.Cells[rowIndex, 15].Value = item.Type;
                    worksheet.Cells[rowIndex, 16].Value = item.ReturnType;
                    worksheet.Cells[rowIndex, 17].Value = item.Identifier;

                    rowIndex++;
                }

                b = p.GetAsByteArray();
            }

            StorageFile file = await targetFolder.CreateFileAsync(_excelFileName, CreationCollisionOption.GenerateUniqueName);

            await FileIO.WriteBytesAsync(file, b);
        }
    }
}
