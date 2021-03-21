﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unage;
using static Unage.ActionData;

namespace Unage
{
    public class DummyData : MonoBehaviour
    {
        static public EventData getDummyEvent1()
        {
            //JSONファイルからイベント情報をロードする
            EventData event1 = new EventData();
            event1.ID = 1;
            event1.Name = "bone";
            List<string> param1 = new List<string>();
            param1.Add("5");
            param1.Add("0.6");
            param1.Add("");
            param1.Add("1.2");

            ActionData action1 = createDummyAction(1, ActionType.Prefeb, PrefabType.FallSprite, "F:/picture/素材/nurusta/pet_hone.png", param1);
            event1.Actions.Add(action1);

            return event1;
        }

        static public EventData getDummyEvent2()
        {
            EventData event2 = new EventData();
            event2.ID = 2;
            event2.Name = "onigiri";
            List<string> param2 = new List<string>();
            param2.Add("5");
            param2.Add("0.4");
            param2.Add("");
            param2.Add("1.2");

            ActionData action2 = createDummyAction(2, ActionType.Prefeb, PrefabType.FallSprite, "F:/picture/素材/nurusta/onigiri_seachicken.png", param2);

            event2.Actions.Add(action2);
            return event2;
        }

        static public EventData getDummyEvent3()
        {
            EventData eventn = new EventData();
            eventn.ID = 3;
            eventn.Name = "jouzu";
            List<string> paramn = new List<string>();
            paramn.Add("6");
            paramn.Add("0.5");
            paramn.Add("");
            paramn.Add("1.2");

            ActionData action3 = createDummyAction(3, ActionType.Prefeb, PrefabType.FallSprite, "F:/picture/素材/nurusta/jouzu.png", paramn);

            eventn.Actions.Add(action3);

            return eventn;
        }

        static public EventData getDummyEvent4()
        {
            EventData eventn = new EventData();
            eventn.ID = 4;
            eventn.Name = "ocha";
            List<string> paramn = new List<string>();
            paramn.Add("5");
            paramn.Add("0.5");
            paramn.Add("");
            paramn.Add("1.2");

            ActionData action4 = createDummyAction(4, ActionType.Prefeb, PrefabType.FallSprite, "F:/picture/素材/nurusta/petbottle_tea.png", paramn);

            eventn.Actions.Add(action4);

            return eventn;
        }

        static public EventData getDummyEvent5()
        {
            EventData eventn = new EventData();
            eventn.ID = 5;
            eventn.Name = "ocha3";
            List<string> paramn = new List<string>();
            paramn.Add("5");
            paramn.Add("0.5");
            paramn.Add("");
            paramn.Add("1.2");

            ActionData action5 = createDummyAction(5, ActionType.Prefeb, PrefabType.FallSprite, "F:/picture/素材/nurusta/petbottle_tea.png", paramn);

            eventn.Actions.Add(action5);
            eventn.Actions.Add(action5);
            eventn.Actions.Add(action5);

            return eventn;
        }

        static public EventData getDummyEvent6()
        {
            EventData eventn = new EventData();
            eventn.ID = 6;
            eventn.Name = "hanataba";
            List<string> paramn = new List<string>();
            paramn.Add("5");
            paramn.Add("0.4");
            paramn.Add("");
            paramn.Add("1.2");

            ActionData action = createDummyAction(6, ActionType.Prefeb, PrefabType.FallSprite, "F:/picture/素材/nurusta/hanataba.png", paramn);

            eventn.Actions.Add(action);

            return eventn;
        }

        static public EventData getDummyEvent7()
        {
            EventData eventn = new EventData();
            eventn.ID = 7;
            eventn.Name = "chiken";
            List<string> paramn = new List<string>();
            paramn.Add("5");
            paramn.Add("0.4");
            paramn.Add("");
            paramn.Add("1.2");

            ActionData action = createDummyAction(7, ActionType.Prefeb, PrefabType.FallSprite, "F:/picture/素材/nurusta/chicken_honetsuki.png", paramn);

            eventn.Actions.Add(action);

            return eventn;
        }

        static public EventData getDummyEvent8()
        {
            EventData eventn = new EventData();
            eventn.ID = 8;
            eventn.Name = "pizza";
            List<string> paramn = new List<string>();
            paramn.Add("5");
            paramn.Add("0.65");
            paramn.Add("");
            paramn.Add("1.2");

            ActionData action = createDummyAction(8, ActionType.Prefeb, PrefabType.FallSprite, "F:/picture/素材/nurusta/food_pizza.png", paramn);

            eventn.Actions.Add(action);

            return eventn;
        }

        static public EventData getDummyEvent9()
        {
            EventData eventn = new EventData();
            eventn.ID = 9;
            eventn.Name = "cake";
            List<string> paramn = new List<string>();
            paramn.Add("5");
            paramn.Add("0.5");
            paramn.Add("");
            paramn.Add("1.2");

            ActionData action = createDummyAction(9, ActionType.Prefeb, PrefabType.FallSprite, "F:/picture/素材/nurusta/christmas_cake.png", paramn);

            eventn.Actions.Add(action);

            return eventn;
        }

        static public EventData getDummyEvent10()
        {
            EventData eventn = new EventData();
            eventn.ID = 10;
            eventn.Name = "sushi";
            List<string> paramn = new List<string>();
            paramn.Add("5");
            paramn.Add("0.35");
            paramn.Add("");
            paramn.Add("1.2");

            ActionData action = createDummyAction(10, ActionType.Prefeb, PrefabType.FallSprite, "F:/picture/素材/nurusta/nigirizushi_moriawase.png", paramn);

            eventn.Actions.Add(action);

            return eventn;
        }

        /// <summary>
        /// ダミー11 草
        /// </summary>
        /// <returns></returns>
        static public EventData getDummyEvent11()
        {
            EventData eventn = new EventData();
            eventn.ID = 11;
            eventn.Name = "kusa";
            List<string> paramn = new List<string>();
            paramn.Add("5");
            paramn.Add("0.8"); //サイズ
            paramn.Add(""); //X位置
            paramn.Add("-0.85"); //Y位置
            paramn.Add(""); //回転
            paramn.Add("0"); //X移動量
            paramn.Add("0"); //Y移動量

            ActionData action = createDummyAction(11, ActionType.Prefeb, PrefabType.MoveSprite, "F:/picture/素材/nurusta/baran.png", paramn);

            eventn.Actions.Add(action);
            eventn.Actions.Add(action);
            eventn.Actions.Add(action);
            eventn.Actions.Add(action);

            return eventn;
        }

        /// <summary>
        /// よっちゃん
        /// </summary>
        /// <returns></returns>
        static public EventData getDummyEvent12()
        {
            EventData eventn = new EventData();
            eventn.ID = 12;
            eventn.Name = "yocchan";
            List<string> paramn = new List<string>();
            paramn.Add("5");
            paramn.Add("0.4"); //サイズ
            paramn.Add(""); //X位置
            paramn.Add(""); //Y位置
            paramn.Add(""); //回転
            paramn.Add("0"); //X移動量
            paramn.Add("0"); //Y移動量

            ActionData action = createDummyAction(12, ActionType.Prefeb, PrefabType.MoveSprite, "F:/picture/素材/nurusta/Yocchan.png", paramn);

            eventn.Actions.Add(action);

            return eventn;
        }

        static public EventData getDummyEvent13()
        {
            EventData eventn = new EventData();
            eventn.ID = 13;
            eventn.Name = "50yen";
            List<string> paramn = new List<string>();
            paramn.Add("5");
            paramn.Add("0.45");
            paramn.Add("");
            paramn.Add("1.2");

            ActionData action = createDummyAction(13, ActionType.Prefeb, PrefabType.FallSprite, "F:/picture/素材/nurusta/money_50.png", paramn);

            eventn.Actions.Add(action);

            return eventn;
        }

        static public EventData getDummyEvent14()
        {
            EventData eventn = new EventData();
            eventn.ID = 14;
            eventn.Name = "100yen";
            List<string> paramn = new List<string>();
            paramn.Add("5");
            paramn.Add("0.45");
            paramn.Add("");
            paramn.Add("1.2");

            ActionData action = createDummyAction(14, ActionType.Prefeb, PrefabType.FallSprite, "F:/picture/素材/nurusta/money_100.png", paramn);

            eventn.Actions.Add(action);

            return eventn;
        }

        /// <summary>
        /// ダミーデータ作成
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="pref"></param>
        /// <param name="path"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        static public ActionData createDummyAction(int id, ActionType type, PrefabType pref, string path, List<string> parameters)
        {
            ActionData action = new ActionData();
            action.ID = id;
            action.Type = type;
            action.Name = pref;
            action.Path = path;
            action.Parameters = parameters;

            return action;
        }

        public static void LoadMockSpachaData(List<SpachaData> SpachaDatas)
        {
            //サンプルデータ2
            SpachaData spa = new SpachaData();
            spa.Amount = new decimal(10);
            spa.EventName = "onigiri";
            SpachaDatas.Add(spa);

            //50：50円玉
            spa = new SpachaData();
            spa.Amount = new decimal(50);
            spa.EventName = "50yen";
            SpachaDatas.Add(spa);

            //51:おにぎり
            spa = new SpachaData();
            spa.Amount = new decimal(51);
            spa.EventName = "onigiri";
            SpachaDatas.Add(spa);

            //53:じょうず星人
            spa = new SpachaData();
            spa.Amount = new decimal(53);
            spa.EventName = "jouzu";
            SpachaDatas.Add(spa);

            //おにぎり
            spa = new SpachaData();
            spa.Amount = new decimal(54);
            spa.EventName = "onigiri";
            SpachaDatas.Add(spa);

            //サンプルデータ1
            spa = new SpachaData();
            spa.Amount = new decimal(1);
            spa.EventName = "bone";
            SpachaDatas.Add(spa);

            //100円だま
            spa = new SpachaData();
            spa.Amount = new decimal(100);
            spa.EventName = "100yen";
            SpachaDatas.Add(spa);

            //サンプルデータ5
            spa = new SpachaData();
            spa.Amount = new decimal(101);
            spa.EventName = "ocha";
            SpachaDatas.Add(spa);

            //サンプルデータ6
            spa = new SpachaData();
            spa.Amount = new decimal(300);
            spa.EventName = "ocha3";
            SpachaDatas.Add(spa);

            //サンプルデータ7
            spa = new SpachaData();
            spa.Amount = new decimal(500);
            spa.EventName = "hanataba";
            SpachaDatas.Add(spa);

            //サンプルデータ8
            spa = new SpachaData();
            spa.Amount = new decimal(1000);
            spa.EventName = "chiken";
            SpachaDatas.Add(spa);

            //サンプルデータ9
            spa = new SpachaData();
            spa.Amount = new decimal(2000);
            spa.EventName = "pizza";
            SpachaDatas.Add(spa);

            //サンプルデータ10
            spa = new SpachaData();
            spa.Amount = new decimal(3000);
            spa.EventName = "cake";
            SpachaDatas.Add(spa);

            //サンプルデータ10
            spa = new SpachaData();
            spa.Amount = new decimal(5000);
            spa.EventName = "sushi";
            SpachaDatas.Add(spa);

            //金額の大きい順にソートする
            SpachaDatas.Sort((a, b) => decimal.Compare(b.Amount, a.Amount));
            Debug.Log(SpachaDatas);
        }

        public static void LoadMockTriggerData(List<TriggerData> triggerDatas)
        {
            //サンプルデータ1 草
            TriggerData tri = new TriggerData();
            tri.Word = "草";
            tri.EventName = "kusa";
            tri.Findtype = TriggerData.FINDTYPE.Partial;
            triggerDatas.Add(tri);

            //サンプルデータ2 よっちゃん
            tri = new TriggerData();
            tri.Word = "よっちゃん";
            tri.EventName = "yocchan";
            tri.Findtype = TriggerData.FINDTYPE.Partial;
            triggerDatas.Add(tri);

        }
    }
}