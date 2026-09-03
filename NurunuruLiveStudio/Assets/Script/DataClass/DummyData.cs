using System.Collections;
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

            ActionData action1 = createDummyAction(1, ActionType.Prefeb, PrefabType.FallSprite, "D:/Assets/nurusta/pet_hone.png", param1);
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

            ActionData action2 = createDummyAction(2, ActionType.Prefeb, PrefabType.FallSprite, "D:/Assets/nurusta/onigiri_seachicken.png", param2);

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

            ActionData action3 = createDummyAction(3, ActionType.Prefeb, PrefabType.FallSprite, "D:/Assets/nurusta/jouzu.png", paramn);

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

            ActionData action4 = createDummyAction(4, ActionType.Prefeb, PrefabType.FallSprite, "D:/Assets/nurusta/petbottle_tea.png", paramn);

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

            ActionData action5 = createDummyAction(5, ActionType.Prefeb, PrefabType.FallSprite, "D:/Assets/nurusta/petbottle_tea.png", paramn);

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

            ActionData action = createDummyAction(6, ActionType.Prefeb, PrefabType.FallSprite, "D:/Assets/nurusta/hanataba.png", paramn);

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

            ActionData action = createDummyAction(7, ActionType.Prefeb, PrefabType.FallSprite, "D:/Assets/nurusta/chicken_honetsuki.png", paramn);

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

            ActionData action = createDummyAction(8, ActionType.Prefeb, PrefabType.FallSprite, "D:/Assets/nurusta/food_pizza.png", paramn);

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

            ActionData action = createDummyAction(9, ActionType.Prefeb, PrefabType.FallSprite, "D:/Assets/nurusta/christmas_cake.png", paramn);

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

            ActionData action = createDummyAction(10, ActionType.Prefeb, PrefabType.FallSprite, "D:/Assets/nurusta/nigirizushi_moriawase.png", paramn);

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

            ActionData action = createDummyAction(11, ActionType.Prefeb, PrefabType.MoveSprite, "D:/Assets/nurusta/baran.png", paramn);

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

            ActionData action = createDummyAction(12, ActionType.Prefeb, PrefabType.MoveSprite, "D:/Assets/nurusta/Yocchan.png", paramn);

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

            ActionData action = createDummyAction(13, ActionType.Prefeb, PrefabType.FallSprite, "D:/Assets/nurusta/money_50.png", paramn);

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

            ActionData action = createDummyAction(14, ActionType.Prefeb, PrefabType.FallSprite, "D:/Assets/nurusta/money_100.png", paramn);

            eventn.Actions.Add(action);

            return eventn;
        }

        /// <summary>
        /// ぷいにゅー
        /// </summary>
        /// <returns></returns>
        static public EventData getDummyEvent15()
        {
            EventData eventn = new EventData();
            eventn.ID = 15;
            eventn.Name = "puinyu";
            List<string> paramn = new List<string>();
            paramn.Add("5");
            paramn.Add("0.4"); //サイズ
            paramn.Add(""); //X位置
            paramn.Add(""); //Y位置
            paramn.Add(""); //回転
            paramn.Add("0"); //X移動量
            paramn.Add("0"); //Y移動量

            ActionData action = createDummyAction(15, ActionType.Prefeb, PrefabType.MoveSprite, "D:/Assets/nurusta/puinyu.png", paramn);

            eventn.Actions.Add(action);

            return eventn;
        }

        /// <summary>
        /// にゃんぱすー
        /// </summary>
        /// <returns></returns>
        static public EventData getDummyEvent16()
        {
            EventData eventn = new EventData();
            eventn.ID = 16;
            eventn.Name = "nyanpasu";
            List<string> paramn = new List<string>();
            paramn.Add("5");
            paramn.Add("0.4"); //サイズ
            paramn.Add(""); //X位置
            paramn.Add(""); //Y位置
            paramn.Add(""); //回転
            paramn.Add("0"); //X移動量
            paramn.Add("0"); //Y移動量

            ActionData action = createDummyAction(16, ActionType.Prefeb, PrefabType.MoveSprite, "D:/Assets/nurusta/nyanpasu.png", paramn);

            eventn.Actions.Add(action);

            return eventn;
        }

        /// <summary>
        /// 黒板消し
        /// </summary>
        /// <returns></returns>
        static public EventData getDummyEvent17()
        {
            EventData eventn = new EventData();
            eventn.ID = 17;
            eventn.Name = "kokuban";
            List<string> paramn = new List<string>();
            paramn.Add("5");
            paramn.Add("0.5"); //サイズ
            paramn.Add(""); //X位置
            paramn.Add("1.2"); //Y位置

            ActionData action = createDummyAction(17, ActionType.Prefeb, PrefabType.FallSprite, "D:/Assets/nurusta/kokubankeshi.png", paramn);

            eventn.Actions.Add(action);

            return eventn;
        }

        /// <summary>
        /// タライ
        /// </summary>
        /// <returns></returns>
        static public EventData getDummyEvent18()
        {
            EventData eventn = new EventData();
            eventn.ID = 18;
            eventn.Name = "tarai";
            List<string> paramn = new List<string>();
            paramn.Add("5");
            paramn.Add("1.0"); //サイズ
            paramn.Add(""); //X位置
            paramn.Add("1.2"); //Y位置

            ActionData action = createDummyAction(18, ActionType.Prefeb, PrefabType.FallSprite, "D:/Assets/nurusta/tarai.png", paramn);

            eventn.Actions.Add(action);

            return eventn;
        }

        /// <summary>
        /// 壺
        /// </summary>
        /// <returns></returns>
        static public EventData getDummyEvent19()
        {
            EventData eventn = new EventData();
            eventn.ID = 19;
            eventn.Name = "tsubo";
            List<string> paramn = new List<string>();
            paramn.Add("5");
            paramn.Add("0.6"); //サイズ
            paramn.Add(""); //X位置
            paramn.Add("1.2"); //Y位置

            ActionData action = createDummyAction(19, ActionType.Prefeb, PrefabType.FallSprite, "D:/Assets/nurusta/tsubo.png", paramn);

            eventn.Actions.Add(action);

            return eventn;
        }

        /// <summary>
        /// モンスターボール
        /// </summary>
        /// <returns></returns>
        static public EventData getDummyEvent20()
        {
            EventData eventn = new EventData();
            eventn.ID = 20;
            eventn.Name = "pokeball";
            List<string> paramn = new List<string>();
            paramn.Add("5");
            paramn.Add("0.25"); //サイズ
            paramn.Add(""); //X位置
            paramn.Add("1.2"); //Y位置

            ActionData action = createDummyAction(20, ActionType.Prefeb, PrefabType.FallSprite, "D:/Assets/nurusta/pokeball.png", paramn);

            eventn.Actions.Add(action);

            return eventn;
        }

        /// <summary>
        /// みがわり
        /// </summary>
        /// <returns></returns>
        static public EventData getDummyEvent21()
        {
            EventData eventn = new EventData();
            eventn.ID = 21;
            eventn.Name = "migawari";
            List<string> paramn = new List<string>();
            paramn.Add("4");
            paramn.Add("0.5"); //サイズ
            paramn.Add(""); //X位置
            paramn.Add("1.2"); //Y位置

            ActionData action = createDummyAction(21, ActionType.Prefeb, PrefabType.FallSprite, "D:/Assets/nurusta/migawari.png", paramn);

            eventn.Actions.Add(action);

            return eventn;
        }

        /// <summary>
        /// ペカコイン1
        /// </summary>
        /// <returns></returns>
        static public EventData getDummyEvent22()
        {
            EventData eventn = new EventData();
            eventn.ID = 22;
            eventn.Name = "pecacoin1";
            List<string> paramn = new List<string>();
            paramn.Add("4");
            paramn.Add("0.8"); //サイズ
            paramn.Add(""); //X位置
            paramn.Add("1.2"); //Y位置

            ActionData action = createDummyAction(22, ActionType.Prefeb, PrefabType.FallSprite, "D:/Assets/nurusta/pecacoin1.png", paramn);

            eventn.Actions.Add(action);

            return eventn;
        }

        /// <summary>
        /// ペカコイン100
        /// </summary>
        /// <returns></returns>
        static public EventData getDummyEvent23()
        {
            EventData eventn = new EventData();
            eventn.ID = 23;
            eventn.Name = "pecacoin100";
            List<string> paramn = new List<string>();
            paramn.Add("4");
            paramn.Add("0.8"); //サイズ
            paramn.Add(""); //X位置
            paramn.Add("1.2"); //Y位置

            ActionData action = createDummyAction(23, ActionType.Prefeb, PrefabType.FallSprite, "D:/Assets/nurusta/pecacoin100.png", paramn);

            eventn.Actions.Add(action);

            return eventn;
        }

        /// <summary>
        /// お団子
        /// </summary>
        /// <returns></returns>
        static public EventData getDummyEvent24()
        {
            EventData eventn = new EventData();
            eventn.ID = 24;
            eventn.Name = "dango";
            List<string> paramn = new List<string>();
            paramn.Add("4");
            paramn.Add("0.6"); //サイズ
            paramn.Add(""); //X位置
            paramn.Add("1.2"); //Y位置

            ActionData action = createDummyAction(24, ActionType.Prefeb, PrefabType.FallSprite, "D:/Assets/nurusta/dango.png", paramn);

            eventn.Actions.Add(action);

            return eventn;
        }

        /// <summary>
        /// ハンバーガー
        /// </summary>
        /// <returns></returns>
        static public EventData getDummyEvent25()
        {
            EventData eventn = new EventData();
            eventn.ID = 25;
            eventn.Name = "hamburger";
            List<string> paramn = new List<string>();
            paramn.Add("4");
            paramn.Add("0.45"); //サイズ
            paramn.Add(""); //X位置
            paramn.Add("1.2"); //Y位置

            ActionData action = createDummyAction(25, ActionType.Prefeb, PrefabType.FallSprite, "D:/Assets/nurusta/hamburger.png", paramn);

            eventn.Actions.Add(action);

            return eventn;
        }

        /// <summary>
        /// うな丼
        /// </summary>
        /// <returns></returns>
        static public EventData getDummyEvent26()
        {
            EventData eventn = new EventData();
            eventn.ID = 26;
            eventn.Name = "unadon";
            List<string> paramn = new List<string>();
            paramn.Add("4");
            paramn.Add("1.50"); //サイズ
            paramn.Add(""); //X位置
            paramn.Add("1.2"); //Y位置

            ActionData action = createDummyAction(26, ActionType.Prefeb, PrefabType.FallSprite, "D:/Assets/nurusta/unadon.png", paramn);

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
            //11:おにぎり
            SpachaData spa = new SpachaData();
            spa.Amount = new decimal(10);
            spa.EventName = "onigiri";
            SpachaDatas.Add(spa);

            //ほね
            spa = new SpachaData();
            spa.Amount = new decimal(2);
            spa.EventName = "bone";
            SpachaDatas.Add(spa);

            //ペカコイン1
            spa = new SpachaData();
            spa.Amount = new decimal(1);
            spa.EventName = "pecacoin1";
            SpachaDatas.Add(spa);

            //おだんご
            spa = new SpachaData();
            spa.Amount = new decimal(50);
            spa.EventName = "dango";
            SpachaDatas.Add(spa);


            //53:じょうず星人
            spa = new SpachaData();
            spa.Amount = new decimal(53);
            spa.EventName = "jouzu";
            SpachaDatas.Add(spa);

            //おだんご
            spa = new SpachaData();
            spa.Amount = new decimal(54);
            spa.EventName = "dango";
            SpachaDatas.Add(spa);

            //おちゃ
            spa = new SpachaData();
            spa.Amount = new decimal(100);
            spa.EventName = "ocha";
            SpachaDatas.Add(spa);

            //おちゃｘ３
            spa = new SpachaData();
            spa.Amount = new decimal(300);
            spa.EventName = "ocha3";
            SpachaDatas.Add(spa);

            //バーガー
            spa = new SpachaData();
            spa.Amount = new decimal(500);
            spa.EventName = "hamburger";
            SpachaDatas.Add(spa);

            //とりにく
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

            //うな丼
            spa = new SpachaData();
            spa.Amount = new decimal(4000);
            spa.EventName = "unadon";
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

            //サンプルデータ3 ぷいにゅー
            tri = new TriggerData();
            tri.Word = "ぷいにゅ";
            tri.EventName = "puinyu";
            tri.Findtype = TriggerData.FINDTYPE.Partial;
            triggerDatas.Add(tri);

            //サンプルデータ4 にゃんぱすー
            tri = new TriggerData();
            tri.Word = "にゃんぱす";
            tri.EventName = "nyanpasu";
            tri.Findtype = TriggerData.FINDTYPE.Partial;
            triggerDatas.Add(tri);
        }
    }
}
