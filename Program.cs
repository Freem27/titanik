using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Titanik
{
    class Program
    {
        static void Main(string[] args)
        {
            string testPath = "test.csv";
            string resultPath="predict.csv";
            //индексы колонок в .csv файле
            int colPassengerId=0, 
                colPclass=1, 
                colName=2, 
                colSex=3, 
                colAge=4, 
                colSibSp=5, 
                colParch=6, 
                colTicket=7, 
                colFare=8, 
                colCabin=9, 
                colEmbarked=10;
            float thresold = 0.5f;

            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            using (StreamWriter writer = File.CreateText(resultPath)) 
            {
                writer.WriteLine("PassengerId,Survived");           //заполняю заголовок
                string[] testRows = File.ReadAllLines(testPath);
                for(int i=1;i<testRows.Count();i++)
                {
                    string[] row = CSVParser.Split(testRows[i]);
                    //Подготовка данных
                    TitanikModel.ModelInput sampleData = new TitanikModel.ModelInput()
                    {
                        Pclass = float.Parse(row[colPclass], CultureInfo.InvariantCulture.NumberFormat),
                        Sex = row[colSex],
                        Age = string.IsNullOrEmpty(row[colAge])?0f:float.Parse(row[colAge], CultureInfo.InvariantCulture.NumberFormat),
                        SibSp = float.Parse(row[colSibSp], CultureInfo.InvariantCulture.NumberFormat),
                        Parch = float.Parse(row[colParch], CultureInfo.InvariantCulture.NumberFormat),
                        Fare = string.IsNullOrEmpty(row[colFare]) ? 0f : float.Parse(row[colFare], CultureInfo.InvariantCulture.NumberFormat),
                        Embarked = row[colEmbarked],
                    };
                    float result = TitanikModel.Predict(sampleData).Score; //Predict

                    string toWrite = $"{row[colPassengerId]},{(result > thresold ? "1" : "0")}";
                    Console.WriteLine(toWrite);
                    writer.WriteLine(toWrite); //если predict > порогового значения - значит выжил
                }
            }
            Console.WriteLine("Завершено");
            Console.ReadKey();
        }
    }
}
