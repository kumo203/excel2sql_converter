using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;

namespace excel2sql_converter // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static readonly string result = "1\n" + @"(1, 'test@example.com', 'GRO-METAL', NULL, NULL, NULL, NULL, '2022-05-18 00:00:00.000000', '2022-05-18 00:00:00.000000', '1234567890', NULL, NULL, NULL, NULL)";
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("処理対象の.xlsxファイルを指定して下さい。");
                    return;
                }

                var target = args[0];

                Console.Write(result);
                //var target = "sample.xlsx";

                //var workbook = WorkbookFactory.Create(target);
                //var sheet = workbook.GetSheetAt(0);
                //var row = sheet.GetRow(0);
                //var cell = row.CreateCell(1);
                //cell.SetCellValue(234);

                //using var stream = new FileStream(target, FileMode.Create);
                //workbook.Write(stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}