using System.Reflection.Emit;

namespace spartaTextRPG
{
    internal class Program
    {
        // 아이템 관련 전역 변수 & 배열
        static public int[] itemBuy = { 0, 0, 0, 0, 0, 0, 0 }; // 구매(보유) 여부 체크 전역 변수
        static public int[] itemEquip = { 0, 0, 0, 0, 0, 0, 0 }; // 각 아이템의 장착 여부 체크 전역 변수
        static public bool equipWeaon = false; // 무기 타입 아이템 장착 여부 체크
        static public bool equipArmor = false; // 방어구 타입 아이템 장착 여부 체크
        static public int? nowWeapon; // 현재 장착 중인 무기 체크용
        static public int? nowArmor; // 현재 장착 중인 방어구 체크용

        static public int[] itemPrice = { 1000, 2250, 3500, 600, 1500, 3750, 100 }; // 아이템 가격
        static public int item_num = 1; // 장착 관리 - 아이템 넘버링용 전역 변수

        // 아이템 효과 관련 전역 변수
        static public bool isStatUpdate = false; // 아이템으로 인한 능력치 증감 최신화 여부 체크
        static public float itemAtk = 0; // 아이템으로 인해 증가한 공격력
        static public float itemDef = 0; // 아이템으로 인해 증가한 방어력

        // 거래 관련 bool 값
        static public bool isStoreBuy = false; // 상점 구매 여부 체크
        static public bool isStoreSell = false; // 상점 판매 여부 체크
        static public bool isRest = false; // 휴식 여부 체크

        // 던전
        static public int dunChoose = 0; // 선택한 던전 번호
        static public string[] dunName = { "쉬운", "일반", "어려운" }; // 던전 이름 목록
        static public int[] dunRequireDef = { 5, 11, 17 }; // 각 던전 요구 방어력
        static public int[] dunReward = { 1000, 1700, 2500 }; // 각 던전 보상 기본값

        // Player 공격력 & 방어력
        

        //===========================================

        class Player // 플레이어 정보
        {
            static public int Level = 1; // 레벨 1 

            static public string Name = "스파르탄"; // 이름
            static public string Job = "전사"; // 직업
            static public double Atk; // 공격력 AtkInfo에서 계산
            static public double Def; // 방어력 DefInfo에서 계산

            static public int HP = 100; // 체력 100 
            static public int Gold = 1500; // 돈 1500

            public void PrintInfo()
            {
                Console.Clear();

                // 안내문
                Console.WriteLine("상태 보기\n캐릭터의 정보가 표시됩니다.\n");

                // 상태 정보
                Console.WriteLine($"Lv. {Level.ToString("D2")}\n{Name} ( {Job} )");
                // Console.WriteLine($"공격력 : {Atk}\n방어력 : {Def}");
                AtkInfo();
                DefInfo();
                Console.WriteLine($"체 력 : {HP}\nGold : {Gold}G\n");

                //선택지
                Console.WriteLine("0. 나가기");

                while (true)
                {
                    // 안내문
                    Console.Write("\n원하시는 행동을 입력해주세요.\n>> ");
                    string input = Console.ReadLine();

                    switch (input)
                    {
                        case "0":
                            {
                                MainInfo();
                                break;
                            }

                        default:
                            {
                                Console.Write("\n잘못된 입력입니다");
                                continue;
                            }
                    }
                }

            }

            public void AtkInfo() // 공격력 표기값 처리
            {
                Atk = (10 + (((double)Level - 1) * 0.5)); // 공격력 10(기본값) + (레벨-1) * 0.5

                if (itemAtk != 0) // 아이템으로 인해 증가한 공격력이 있다면
                {
                    Console.WriteLine($"공격력 : {Atk + itemAtk} (+{itemAtk})");
                }
                else // 없다면
                {
                    Console.WriteLine($"공격력 : {Atk}");
                }
            }
            public void DefInfo() // 공격력 표기값 처리
            {
                Def = (5 + (((double)Level - 1)) * 1); // 방어력 5(기본값) + (레벨-1) * 1

                if (itemDef != 0) // 아이템으로 인해 증가한 방어력이 있다면
                {
                    Console.WriteLine($"방어력 : {Def + itemDef} (+{itemDef})");
                }
                else // 없다면
                {
                    Console.WriteLine($"방어력 : {Def}");
                }
            }

        }

        //===========================================

        static public void Inventory() // 인벤토리
        {
            Console.Clear();

            Console.WriteLine("인벤토리\n보유 중인 아이템을 관리할 수 있습니다.\n\n[아이템 목록]");

            for (int i = 0; i < itemBuy.Length; i++) // 보유 아이템 정보 출력
            {
                if (itemBuy[i] == 1)
                {
                    itemEquipMark(i);
                    item_invenInfo(i);
                    Console.WriteLine(); // 목록 줄바꿈
                }
            }

            // 인벤토리 선택지
            Console.WriteLine("\n1. 장착 관리\n0. 나가기\n");

            while (true)
            {
                // 인벤토리 안내문
                Console.Write("원하시는 행동을 입력해주세요.\n>> ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "0":
                        {
                            MainInfo(); // 메인 화면으로 복귀
                            break;
                        }

                    case "1":
                        {
                            Equipt(); // 장착 관리 진입
                            break;
                        }

                    default:
                        {
                            Console.WriteLine("\n잘못된 입력입니다");
                            continue;
                        }
                }
            }
        }

        //===========================================

        static public void Equipt() // 장착 관리
        {
            int[] itemlist = new int[itemBuy.Length]; // 목록 순서 번호와 실제 아이템 번호 연동용 임시 배열

            Console.Clear();
            Console.WriteLine("인벤토리 - 장착 관리\n보유 중인 아이템을 관리할 수 있습니다.\n\n[아이템 목록]");

            for (int i = 0; i < itemBuy.Length; i++) // 보유 아이템 정보 출력
            {
                if (itemBuy[i] == 1)
                {
                    itemEquipMarkNum(i);
                    item_invenInfo(i);
                    Console.WriteLine(); // 목록 줄바꿈

                    // ex) 목록 0번에 4번 아이템이 들어오면 itemlist[1-1] = 4
                    // 이후 equiptItem[]에 해당 i값을 넣을 예정
                    itemlist[item_num - 1] = i;

                    item_num += 1; // 이후 목록에 등장할 아이템 넘버링
                }
            }

            int itemCount = item_num - 1; // 목록에 나타난 아이템 개수 체크
            item_num = 1; // 아이템 넘버링 종료 후 초기화

            // 장착 관리 선택지
            Console.WriteLine("\n0. 나가기\n");

            while (true)
            {
                // 장착 관리 안내문
                Console.Write("원하시는 행동을 입력해주세요.\n>> ");
                string input = Console.ReadLine();

                //int intput = int.Parse(input); // 입력값의 int형

                bool isNumberCheck = int.TryParse(input, out int intput); // 입력값이 숫자가 아니라면 -1로 설정
                {
                    if (isNumberCheck)
                    {
                        intput = int.Parse(input); // 입력값의 int형
                    }
                    else
                    {
                        intput = -1;
                    }
                }

                if (intput == 0) // 메인 화면으로 복귀
                {
                    MainInfo();
                    break;
                }
                else if (intput >= 1 && intput <= itemCount) // intput이 1 이상이며 목록 내 아이템 개수 이하일 때 - 즉, 아이템 장착 또는 해제
                {
                    isStatUpdate = false; // 아이템 장착 여부가 변경되었으므로 능력치 반영 최신화 시켜줘야 함
                    int number;
                    number = itemlist[intput - 1]; // intput - 1은 호출하려는 itemlist의 번호, 이를 임시 변수에 보관

                    switch (itemEquip[number])
                    {
                        case 0:
                            itemEquip[number] = 1; // 아이템 장착

                            // Q. 장착 중인 장비 중 무기 or 방어구가 있단 건 어떻게 판단해야 하는가
                            // A. 보유 중인 아이템 목록을 순차적을 체크 - 장착 중인가? - type이 Weapon(or Armor)인가?
                            // 아이템의 클래스화 필요 - 시간 부족

                            // 현재 장착한 장비가 있다면 nowWeapon or nowArmor가 null이 아니라면
                            // itemEquipt[nowWeapon or nowArmor]를 0으로 전환, 이후 현재 아이템을 등록

                            // 현재 장착한 장비가 없다면, null 이라면
                            // 현재 아이템을 등록

                            Equipt();
                            break;

                        case 1:
                            itemEquip[number] = 0; // 아이템 장착 해제
                            Equipt();
                            break;

                        default:
                            Console.Write("장착 관리에서 오류");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("\n잘못된 입력입니다.");
                    continue;
                }
            }
        }

        //===========================================

        static public void itemEquipMark(int i) // 인벤토리 내 장착 여부 표시
        {
            if (itemEquip[i] == 1)
            {
                Console.Write($"- [E]");
            }
            else if (itemEquip[i] == 0)
            {
                Console.Write($"- ");
            }
        }

        //===========================================

        static public void itemEquipMarkNum(int i) // 장착 관리 내 장착 여부 표시
        {
            if (itemEquip[i] == 1)
            {
                Console.Write($"- {item_num} [E]");
            }
            else if (itemEquip[i] == 0)
            {
                Console.Write($"- {item_num} ");
            }
        }

        //===========================================

        static public void item_invenInfo(int i) // 아이템 정보
        {
            switch (i)
            {
                case 0: // 수련자 갑옷
                    {
                        Console.Write("수련자 갑옷\t| 방어력 +5 | 수련에 도움을 주는 갑옷입니다.\t");
                        break;
                    }

                case 1: // 무쇠 갑옷
                    {
                        Console.Write("무쇠 갑옷\t| 방어력 +9 | 무쇠로 만들어져 튼튼한 갑옷입니다.\t");
                        break;
                    }
                case 2: // 스파르타의 갑옷
                    {
                        Console.Write("스파르타의 갑옷\t| 방어력 +15 | 스파르타의 전사들이 사용했다는 전설의 갑옷입니다.\t");
                        break;
                    }
                case 3: // 낡은 검
                    {
                        Console.Write("낡은 검\t| 공격력 +2 | 쉽게 볼 수 있는 낡은 검 입니다.\t");
                        break;
                    }
                case 4: // 청동 도끼
                    {
                        Console.Write("청동 도끼\t| 공격력 +5 | 어디선가 사용됐던거 같은 도끼입니다.\t");
                        break;
                    }
                case 5: // 스파르타의 창
                    {
                        Console.Write("스파르타의 창\t| 공격력 +7 | 스파르타의 전사들이 사용했다는 전설의 창입니다.\t");
                        break;
                    }
                case 6: // 도전 기능 - 나만의 아이템
                    {
                        Console.Write("창작 무기\t| 공격력 +10 | 창작 무기 입니다.\t");
                        break;
                    }
                default:
                    {
                        Console.Write("인벤토리 - 아이템 구매 여부 체크쪽 에러");
                        break;
                    }
            }
        }

        //===========================================

        static public void itemPriceInfoBuy(int i) // 아이템 가격 정보 (구입 화면)
        {
            if (itemBuy[i] == 0) // 아이템을 구입하지 않았다면
            {
                Console.WriteLine($"| {itemPrice[i]}G");
            }

            else if (itemBuy[i] == 1) // 구입한 아이템이라면
            {
                Console.WriteLine("|    구매완료");
            }

            else
            {
                Console.WriteLine("상점 가격 표시 체크쪽 에러");
            }

        }

        //===========================================

        static public void itemPriceInfoSell(int i) // 아이템 가격 정보 (판매 화면)
        {
            Console.WriteLine($"| {itemPrice[i]}G");
        }

        //===========================================

        static public void itemStat(int i) // 각 아이템의 능력치 증감값
        {
            switch (i)
            {
                case 0: // 수련자 갑옷
                    {
                        itemDef += 5;
                        break;
                    }

                case 1: // 무쇠 갑옷
                    {
                        itemDef += 9;
                        break;
                    }
                case 2: // 스파르타의 갑옷
                    {
                        itemDef += 15;
                        break;
                    }
                case 3: // 낡은 검
                    {
                        itemAtk += 2;
                        break;
                    }
                case 4: // 청동 도끼
                    {
                        itemAtk += 5;
                        break;
                    }
                case 5: // 스파르타의 창
                    {
                        itemAtk += 7;
                        break;
                    }
                case 6: // 도전 기능 - 나만의 아이템
                    {
                        itemAtk += 10;
                        break;
                    }
                default:
                    {
                        Console.Write("아이템 공격력/방어력 최신화에서 오류");
                        break;
                    }
            }
        }

        //===========================================

        static public void itemStatUpdate() // 아이템으로 인한 능력치 증감값 최신화
        {
            // 아이템으로 인한 증감값 초기화 후 재계산
            itemAtk = 0;
            itemDef = 0;

            for (int i = 0; i < itemEquip.Length; i++)
            {
                if (itemEquip[i] == 1) // 아이템이 장착되어있다면
                {
                    itemStat(i); // 그에 따른 능력치를 증감
                }
            }

            isStatUpdate = true; // 최신화 완료 처리
        }

        //===========================================

        static public void StoreInfo() // 상점 - 정보
        {
            Console.Clear();

            // 상점 안내문
            Console.WriteLine("상점\n필요한 아이템을 얻을 수 있는 상점입니다.\n");
            Console.WriteLine("[보유 골드]");
            Console.WriteLine($"{Player.Gold}G\n");

            Console.WriteLine("[아이템 목록]");

            for (int i = 0; i < itemBuy.Length; i++) // 판매 아이템 목록 표시
            {
                //Console.Write($"- {i} ");
                Console.Write($"- ");
                item_invenInfo(i);
                itemPriceInfoBuy(i);
            }

            // 상점 선택지
            Console.WriteLine("\n1. 아이템 구매\n2. 아이템 판매\n0. 나가기\n");

            while (true)
            {
                // 상점 안내문
                Console.Write("원하시는 행동을 입력해주세요.\n>> ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "0":
                        {
                            MainInfo(); // 메인 화면으로 복귀
                            break;
                        }

                    case "1":
                        {
                            StoreTrade(); // 아이템 구매 진입
                            break;
                        }

                    case "2":
                        {
                            StoreSell(); // 아이템 판매 진입
                            break;
                        }

                    default:
                        {
                            Console.Write("\n잘못된 입력입니다");
                            continue;
                        }
                }
            }
        }

        //===========================================

        static public void StoreTrade() // 상점 - 아이템 구매
        {
            Console.Clear();

            // 상점 - 아이템 구매 안내문
            Console.WriteLine("상점 - 아이템 구매\n필요한 아이템을 얻을 수 있는 상점입니다.\n");
            Console.WriteLine("[보유 골드]");
            Console.WriteLine($"{Player.Gold}G\n");

            Console.WriteLine("[아이템 목록]");

            for (int i = 0; i < itemBuy.Length; i++) // 판매 아이템 목록 표시
            {
                Console.Write($"- {i + 1} ");
                item_invenInfo(i);
                itemPriceInfoBuy(i);
            }

            // 상점 선택지
            Console.WriteLine("\n0. 나가기\n");

            if (isStoreBuy == true) // 아이템을 구매해서 목록이 갱신된 경우
            {
                Console.WriteLine("구매가 완료되었습니다.");

                isStoreBuy = false; // 구매 여부 초기화
            }

            while (true)
            {
                // 상점 안내문
                Console.Write("원하시는 행동을 입력해주세요.\n>> ");
                string input = Console.ReadLine();

                // int intput = int.Parse(input); // input의 int형

                bool isNumberCheck = int.TryParse(input, out int intput); // 입력값이 숫자가 아니라면 -1로 설정
                {
                    if (isNumberCheck)
                    {
                        intput = int.Parse(input); // 입력값의 int형
                    }
                    else
                    {
                        intput = -1;
                    }
                }

                if (intput == 0)
                {
                    // MainInfo();
                    StoreInfo(); // 상점 정보 화면으로 복귀
                    break;
                }
                else if (intput >= 1 && intput <= itemBuy.Length)
                {
                    if (itemBuy[intput - 1] == 1) // 이미 구매한 아이템일 경우
                    {
                        Console.WriteLine("\n이미 구매한 아이템입니다.");
                        continue;
                    }

                    else if (itemBuy[intput - 1] == 0 && Player.Gold >= itemPrice[intput - 1]) // 미구입한 아이템 선택 & 소지금 충분
                    {
                        Player.Gold -= itemPrice[intput - 1]; // 소지금에서 아이템 가격 차감
                        itemBuy[intput - 1] = 1; // 아이템 구매 처리

                        isStoreBuy = true; // 구매 여부 활성화
                        StoreTrade(); // 구입 정보창 최신화                      
                        break;
                    }

                    else if (itemBuy[intput - 1] == 0 && Player.Gold < itemPrice[intput - 1]) // 미구입한 아이템 선택 & 소지금 부족
                    {
                        Console.WriteLine("\nGold 가 부족합니다.");
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine("\n잘못된 입력입니다.\n");
                    continue;
                }
            }
        }

        //===========================================

        static public void StoreSell() // 상점 - 아이템 판매
        {
            int[] itemlist = new int[itemBuy.Length]; // 목록 순서 번호와 실제 아이템 번호 연동용 임시 배열

            Console.Clear();

            // 상점 - 아이템 판매 안내문
            Console.WriteLine("상점 - 아이템 판매\n필요한 아이템을 얻을 수 있는 상점입니다.\n");
            Console.WriteLine("[보유 골드]");
            Console.WriteLine($"{Player.Gold}G\n");

            Console.WriteLine("[아이템 목록]");

            for (int i = 0; i < itemBuy.Length; i++) // 보유 아이템 정보 출력
            {
                if (itemBuy[i] == 1)
                {
                    itemEquipMarkNum(i); // 장착 여부가 판매 결정에 영향을 줄 수 있다 판단하여 존치
                    item_invenInfo(i);
                    itemPriceInfoSell(i);
                    Console.WriteLine(); // 목록 줄바꿈

                    // ex) 목록 0번에 4번 아이템이 들어오면 itemlist[1-1] = 4
                    // 이후 equiptItem[]에 해당 i값을 넣을 예정
                    itemlist[item_num - 1] = i;

                    item_num += 1; // 이후 목록에 등장할 아이템 넘버링
                }
            }

            int itemCount = item_num - 1; // 목록에 나타난 아이템 개수 체크
            item_num = 1; // 아이템 넘버링 종료 후 초기화

            // 아이템 판매 선택지
            Console.WriteLine("\n0. 나가기\n");

            if (isStoreSell == true) // 아이템을 판매해서 목록이 갱신된 경우
            {
                Console.WriteLine("판매가 완료되었습니다.");

                isStoreSell = false; // 판매 여부 초기화
            }

            while (true)
            {
                // 아이템 판매 안내문
                Console.Write("원하시는 행동을 입력해주세요.\n>> ");
                string input = Console.ReadLine();

                bool isNumberCheck = int.TryParse(input, out int intput); // 입력값이 숫자가 아니라면 -1로 설정
                {
                    if (isNumberCheck)
                    {
                        intput = int.Parse(input); // 입력값의 int형
                    }
                    else
                    {
                        intput = -1;
                    }
                }

                if (intput == 0) // 상점 정보 화면으로 복귀
                {
                    // MainInfo();
                    StoreInfo();
                    break;
                }
                else if (intput >= 1 && intput <= itemCount) // intput이 1 이상이며 목록 내 아이템 개수 이하일 때 - 즉, 목록의 아이템 선택
                {
                    isStatUpdate = false; // 아이템 보유 여부가 변경되었으므로 능력치 반영 최신화 시켜줘야 함
                    int number;
                    number = itemlist[intput - 1]; // intput - 1은 호출하려는 itemlist의 번호, 이를 임시 변수에 보관

                    itemEquip[number] = 0; // 장착 여부 초기화
                    itemBuy[number] = 0; // 보유 여부 초기화

                    double dbPrice = itemPrice[intput - 1]; // 판매가 - 구입 가격의 85% 계산용 임시 변수
                    double sellPrice = dbPrice * 0.85; // 구입 가격의 85% 계산

                    Player.Gold += (int)sellPrice; // 판매가 획득 // 명시적 형전환

                    isStoreSell = true; // 구매 여부 활성화
                    StoreSell(); // 판매 정보창 최신화
                    break;
                }
                else
                {
                    Console.WriteLine("\n잘못된 입력입니다.");
                    continue;
                }
            }

        }

        //===========================================

        static public void Rest() // 휴식
        {
            Console.Clear();

            // 휴식 안내문
            Console.WriteLine($"500 G 를 내면 체력을 회복할 수 있습니다. (보유 골드 : {Player.Gold} G)");

            // 휴식 선택지
            Console.WriteLine("\n1. 휴식하기\n0. 나가기\n");

            if (isRest == true) // 휴식 시 출력
            {
                Console.WriteLine("휴식을 완료했습니다.");

                isRest = false; // 출력 여부 초기화
            }


            while (true)
            {
                Console.Write("원하시는 행동을 입력해주세요.\n>> ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "0":
                        {
                            MainInfo();
                            break;
                        }
                    case "1":
                        {
                            if (Player.Gold >= 500)
                            {
                                isRest = true;
                                Player.HP = 100;
                                Player.Gold -= 500;
                                Rest();
                                break;
                            }
                            else if (Player.Gold < 500)
                            {
                                Console.WriteLine("\nGold 가 부족합니다.");
                                continue;
                            }
                            else
                            {
                                Console.WriteLine("휴식에 필요한 골드 체크에서 오류.");
                                continue;
                            }
                        }
                    default:
                        {
                            Console.WriteLine("\n잘못된 입력입니다.");
                            continue;
                        }

                }
            }
        }

        //===========================================

        static public void DunInfo() // 던전입장 화면
        {
            Console.Clear();

            // 상점 - 던전입장 안내문
            Console.WriteLine("던전입장\n이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.\n");
            for (int i = 0; i < dunName.Length; i++) // 던전 목록 출력
            {
                Console.WriteLine($"{i + 1}. {dunName[i]} 던전\t| 방어력 {dunRequireDef[i]} 이상 권장");
            }
            Console.WriteLine("0. 나가기");

            while (true)
            {
                Console.Write("\n원하시는 행동을 입력해주세요.\n>> ");
                string input = Console.ReadLine();

                bool isNumberCheck = int.TryParse(input, out int intput); // 입력값이 숫자가 아니라면 -1로 설정
                {
                    if (isNumberCheck)
                    {
                        intput = int.Parse(input); // 입력값의 int형
                    }
                    else
                    {
                        intput = -1;
                    }
                }

                if (intput == 0) // 메인화면으로 복귀
                {
                    MainInfo();
                    break;
                }
                else if (intput >= 1 && intput <= 3) // 던전 선택 (임시)
                {
                    dunChoose = intput - 1; // 선택한 던전 번호 저장
                    DunClear();
                    break;
                }
                else
                {
                    Console.WriteLine("\n잘못된 입력입니다.");
                }
            }
        }

        //===========================================

        static public void DunClear() // 던전 클리어 화면
        {
            Console.Clear();

            // 던전 클리어 안내문
            Console.WriteLine($"던전 클리어\n축하합니다!!\n{dunName[dunChoose]}던전을 클리어 하였습니다.\n");

            // 탐험 결과
            Console.WriteLine($"[탐험 결과]\n체력 {Player.HP} -> {Player.HP - 30}\nGold {Player.Gold} G -> {Player.Gold + dunReward[dunChoose]} G");

            Console.WriteLine("\n0. 나가기");

            Player.HP -= 30; // 체력 감소 (임시)
            Player.Gold += dunReward[dunChoose]; // 보상 지급 (임시)
            dunChoose = 0; // 선택한 던전 번호 초기화
            Player.Level += 1; // 레벨업

            while (true)
            {
                Console.Write("\n원하시는 행동을 입력해주세요.\n>> ");
                string input = Console.ReadLine();

                if (input == "0")
                {
                    DunInfo(); // 던전 선택 화면으로 복귀
                    break;
                }
                else
                {
                    Console.WriteLine("\n잘못된 입력입니다.");
                    continue;
                }

            }

        }

        //===========================================

        static public void MainInfo() // 시작 화면
        {
            Console.Clear();

            if (isStatUpdate != true) // 아이템 능력치 증값값 최신화 여부 체크
            {
                itemStatUpdate(); // 안되어있으면 최신화 처리
            }

            Player player = new Player();

            // 안내문
            Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
            Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.\n");

            // 선택지
            Console.WriteLine("1. 상태 보기\n2. 인벤토리\n3. 상점\n4. 던전입장\n5. 휴식\n");

            while (true)
            {
                // 플레이어의 행동 입력
                Console.Write("원하시는 행동을 입력해주세요.\n>> ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        player.PrintInfo();
                        break;

                    case "2":
                        Inventory();
                        break;

                    case "3":
                        StoreInfo();
                        break;

                    case "4":
                        DunInfo();
                        break;

                    case "5":
                        Rest();
                        break;

                    default:
                        Console.WriteLine("\n잘못된 입력입니다");
                        continue;
                }
            }

        }

        //===========================================

        static void Main(string[] args) // main
        {
            MainInfo();
        }
    }
}
