using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Trabo.Model;

namespace Trabo.History
{
    public class HistoricalDataReader
    {
        public async Task<IReadOnlyList<HistoricalDataModel>> ReadData(string path)
        {
            var data = new List<HistoricalDataModel>(3000000);
            using (var file = File.OpenRead(path))
            using (var reader = new StreamReader(file))
            {
                reader.ReadLine(); // Timestamp,Open,High,Low,Close,Volume_(BTC),Volume_(Currency),Weighted_Price

                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    var values = line.Split(',');
                    var model = new HistoricalDataModel
                    {
                        Date = DataExtensions.ToDateTime(long.Parse(values[0]), seconds: true),
                        Open = decimal.Parse(values[1]),
                        Hight = decimal.Parse(values[2]),
                        Low = decimal.Parse(values[3]),
                        Close = decimal.Parse(values[4]),
                      //  BTCVolume = double.Parse(values[5]),
                      //  USDVolume = double.Parse(values[6]),
                      //  WeightedPrice = decimal.Parse(values[7]),
                    };

                    data.Add(model);
                }
            }

            return data;
        }
    }

    public struct HistoricalDataModel
    {
        public DateTime Date;
        public decimal Open;
        public decimal Hight;
        public decimal Low;
        public decimal Close;
        public double BTCVolume;
        public double USDVolume;
        public decimal WeightedPrice;

        public override string ToString()
        {
            return $"Timestamp:{Date}, Open:{Open}, High:{Hight}, Low:{Low} ,Close:{Close}";//, Volume_(BTC):{BTCVolume}, Volume_(Currency): {USDVolume} ,Weighted Price:{WeightedPrice}";
        }
    }
}
