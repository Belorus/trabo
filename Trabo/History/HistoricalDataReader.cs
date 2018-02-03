using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Trabo.Model;

namespace Trabo.History
{
    public class HistoricalDataReader : IDisposable
    {
        private StreamReader _reader;


        public void OpenData(string path)
        {
            var file = File.OpenRead(path);
            _reader = new StreamReader(file);
            _reader.ReadLine(); // Timestamp,Open,High,Low,Close,Volume_(BTC),Volume_(Currency),Weighted_Price
        }

        public IEnumerable<HistoricalDataModel> Data()
        {
            string line;
            while ((line = _reader.ReadLine()) != null)
            {
                var values = line.Split(',');
                var model = new HistoricalDataModel
                {
                    Date = long.Parse(values[0]),
                    Open = double.Parse(values[1]),
                    Hight = double.Parse(values[2]),
                    Low = double.Parse(values[3]),
                    Close = double.Parse(values[4]),
                    BTCVolume = double.Parse(values[5]),
                    USDVolume = double.Parse(values[6]),
                    //  WeightedPrice = decimal.Parse(values[7]),
                };

                yield return model;
            }
        }
        
        public void Dispose()
        {
            _reader.Dispose();
        }
    }

    public class HistoricalDataModel
    {
        public long Date;
        public double Open;
        public double Hight;
        public double Low;
        public double Close;
        public double BTCVolume;
        public double USDVolume;
        public decimal WeightedPrice;

        public override string ToString()
        {
            return $"Timestamp:{Date}, Open:{Open}, High:{Hight}, Low:{Low} ,Close:{Close}";//, Volume_(BTC):{BTCVolume}, Volume_(Currency): {USDVolume} ,Weighted Price:{WeightedPrice}";
        }
    }
}
