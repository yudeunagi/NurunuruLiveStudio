using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Unage
{
    public class PayedMoney
    {
        // 通貨タイプ
        public enum CurrencyType : int
        {
            Yen = 1,
            Doller = 2,
        }

        //通貨
        private int currency;

        //金額
        private decimal amount;

        public int CURRENCY
        {
            //getアクセサ
            get { return currency; }

            //setアクセサ
            set { currency = value; }
        }

        public decimal AMOUNT
        {
            //getアクセサ
            get { return amount; }

            //setアクセサ
            set { amount = value; }
        }
    }
}
