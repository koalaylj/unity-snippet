using UnityEngine;
using System.Collections.Generic;
using System;
using SimpleJson;

public class HeroBattlePresenter : Presenter
{
    [SerializeField]
    private GameObject _heroCardPrefab;

    [SerializeField]
    private HeroIconComponent[] _heroIcon;

    [SerializeField]
    private Transform[] _heroCardAnchor;

    [SerializeField]
    private UILabel _teamNameLabel;

    [SerializeField]
    private GameObject _pre;

    [SerializeField]
    private GameObject _next;

    [SerializeField]
    private GameObject _prePage;

    [SerializeField]
    private GameObject _nextPage;

    [SerializeField]
    private UILabel _pageLabel;

    [SerializeField]
    private GameObject _return;

    [SerializeField]
    private GameObject _debug_add_hero;

    private string[] _teamName = { "队伍一", "队伍二", "队伍三" };

    private int _curTeamIndex = 0;
    //   private int _teamIndex = 0;


    //当前页 从0开始索引
    private int _pageIndex = 0;

    //总页数
    private int _totalPage = 0;

    // 英雄中存储的是Dictionary
    private List<STUHeroInfo> _heroList = new List<STUHeroInfo>();


    //分页 每页显示的数目
    private const int _PER_PAGE_COUNT = 4;


    void Start()
    {

        UIEventListener.Get(_return).onClick = (sender) =>
        {
            UIManager.Instance.Hide(this);
            UIManager.Instance.Show("Panel_MainInterface");
        };


        UIEventListener.Get(_pre).onClick = (sender) =>
        {
            ChangeTeam(_curTeamIndex - 1);
        };

        UIEventListener.Get(_next).onClick = (sender) =>
        {
            ChangeTeam(_curTeamIndex + 1);
        };

        UIEventListener.Get(_prePage).onClick = (sender) =>
        {
            PrePage();

        };

        UIEventListener.Get(_nextPage).onClick = (sender) =>
        {
            NextPage();
        };

        UIEventListener.Get(_debug_add_hero).onClick = (sender) =>
        {
            ProCenter.Instance.DebugCreateHero((mes) =>
            {
                if (mes.Success)
                {
                    HeroDomain.Instance.AddHeroOrChips(mes.data);
    //                HeroDomain.Instance.AllHeroDict.Add(mes.data.id, mes.data);
                    OnShown();
                }
                else
                {
                    UIManager.Instance.ShowError(mes.code);
                }
            });
        };


        foreach (var item in _heroIcon)
        {
            item.OnClickEvent = OnTeamHeroClickHandler;
        }
    }


    public override void OnShown()
    {
        _pageIndex = 0;

        RefreshHeros();

        SetPageLabel();

        ChangeTeam(0, true);

        ShowPage(0);
    }

    private void SetPageButtonState()
    {
        bool isFirstPage = _pageIndex <= 0;
        _prePage.SetActive(!isFirstPage);

        bool isLastPage = _pageIndex >= _totalPage - 1;
        _nextPage.SetActive(!isLastPage);

    }

    private void RefreshHeros()
    {
        _heroList = SortHeroes(HeroDomain.Instance.AllHeroDict.Values);
        _totalPage = _heroList.Count % 4 == 0 ? _heroList.Count / 4 : _heroList.Count / 4 + 1;

        if (_pageIndex > _totalPage - 1)
        {
            _pageIndex = _totalPage - 1;
            if (_pageIndex < 0)
            {
                _pageIndex = 0;
            }
        }
    }

    /// <summary>
    /// 排序英雄显示顺序
    /// 优先按照队伍情况排列，队伍1》队伍2》队伍3》没队伍的
    /// 同队伍，按照队伍内顺序排列，1》2》3》4
    /// 没队伍的，先按照品质排列
    /// 同品质，按照转生次数从高到低排列
    /// 同转生次数，按照等级从高到低排列
    /// 
    /// </summary>
    /// <param name="heroes"></param>
    /// <returns>排序的英雄链表</returns>
    private List<STUHeroInfo> SortHeroes(ICollection<STUHeroInfo> heroes)
    {
        List<STUHeroInfo> temp = new List<STUHeroInfo>();
        temp.AddRange(heroes);

        List<STUHeroInfo> sorted = new List<STUHeroInfo>();

        sorted.AddRange(HeroDomain.Instance.GetTeamMembers(0));
        sorted.AddRange(HeroDomain.Instance.GetTeamMembers(1));
        sorted.AddRange(HeroDomain.Instance.GetTeamMembers(2));

        foreach (var item in sorted)
        {
            temp.Remove(item);
        }


        temp.Sort((hero1, hero2) =>
        {
            int compare = hero2.grade - hero1.grade;

            if (0 == compare)
            {
                compare = hero2.rebirth - hero1.rebirth;

                if (0 == compare)
                {
                    compare = hero2.Level - hero1.Level;

                    if (compare == 0)
                    {
                        return hero1.originType - hero2.originType;
                    }
                    return compare;
                }

                return compare;


            }

            return compare;
        });

        sorted.AddRange(temp);

        return sorted;
    }

    private void SetPageLabel()
    {
        _pageLabel.text = string.Format("[00fff6]{0}[-]/{1}", _pageIndex + 1, _totalPage);
    }

    public void ChangeTeam(int teamIndex, bool refresh = true)
    {
        _curTeamIndex = teamIndex;
        if (_curTeamIndex < 0)
        {
            _curTeamIndex = 2;
        }
        else if (_curTeamIndex > 2)
        {
            _curTeamIndex = 0;
        }
        ShowTeam(_curTeamIndex);
        SelectIt(null);
        if (refresh)
        {
            RefreshAllHeroes();
        }
    }


    private void OnTeamHeroClickHandler(KIconComponent sender)
    {
        SelectIt(sender);
        RefreshAllHeroes();
    }

    private void RefreshAllHeroes()
    {
        foreach (var item in _heroCardAnchor)
        {
            HeroCardComponent com = item.GetComponentInChildren<HeroCardComponent>();
            if (com != null)
            {
                com.SetState(_selected, _curTeamIndex);
            }
        }
    }

    /// <summary>
    /// 根据team index 显示team 英雄信息
    /// </summary>
    /// <param name="index">team index</param>
    private void ShowTeam(int index)
    {
        List<STUHeroInfo> teamMembers = HeroDomain.Instance.GetTeamMembers(index);

        foreach (var item in _heroIcon)
        {
            item.DataContent = null;
        }

        for (int i = 0; i < teamMembers.Count; i++)
        {
            var hero = teamMembers[i];
            var heroRes = HeroDomain.Instance.GetHeroResByType(hero.originType);
            _heroIcon[i].DataContent = hero;
            _heroIcon[i].Focused = false;
            _selected = null;
        }

        _teamNameLabel.text = _teamName[index];
    }

    KIconComponent _selected = null;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    private void SelectIt(KIconComponent item)
    {
        if (item == null)
        {
            if (_selected != null)
            {
                _selected.Focused = false;
            }
            return;
        }

        if (_selected != null && _selected == item)
        {
            _selected.Focused = true;
            return;
        }

        if (_selected != null)
        {
            _selected.Focused = false;
        }

        _selected = item;
        _selected.Focused = true;
    }

    /// <summary>
    /// 未上阵的英雄 替换上阵的英雄
    /// </summary>
    /// <param name="com"></param>
    private void OnReplaceEventHandler(HeroCardComponent com)
    {
        STUHeroInfo retired = _selected.DataContent as STUHeroInfo;
        int teamIndex = -1;
        int heroIndexInTeam = -1;
        HeroDomain.Instance.GetHeroIndex(retired.id, ref teamIndex, ref heroIndexInTeam);

        STUHeroInfo fresh = com.DataContent as STUHeroInfo;


        UIManager.Instance.BeginWaiting();
        ProCenter.Instance.ReqHeroJoinTeamReplaceSb(fresh.id, retired.id, teamIndex + 1, (msg) =>
        {
            UIManager.Instance.EndWaiting();
            if (msg.Success)
            {
                HeroDomain.Instance.ReplaceTeamMember(fresh.id, teamIndex, heroIndexInTeam);

                ShowTeam(_curTeamIndex);
                ShowPage(_pageIndex);
            }
            else
            {
                UIManager.Instance.ShowError(msg.code);
            }
        });
    }

    /// <summary>
    /// 上阵英雄站位互换
    /// </summary>
    /// <param name="com"></param>
    private void OnChangePositionEventHandler(HeroCardComponent com)
    {
        if (_selected == null)
        {
            throw new Exception("此处不应该能点击交换位置 谢特啊!");
        }

        string heroId1 = com.DataContent.id;
        int heroTeamIndex1 = -1;
        int heroIndexInTeam1 = -1;
        HeroDomain.Instance.GetHeroIndex(heroId1, ref heroTeamIndex1, ref heroIndexInTeam1);

        STUHeroInfo hero2 = _selected.DataContent as STUHeroInfo;
        int heroTeamIndex2 = -1;
        int heroIndexInTeam2 = -1;
        HeroDomain.Instance.GetHeroIndex(hero2.id, ref heroTeamIndex2, ref heroIndexInTeam2);

        UIManager.Instance.BeginWaiting();

        ProCenter.Instance.ReqHeroChangeTeamPosition(heroId1, heroTeamIndex1 + 1, hero2.id, heroTeamIndex2 + 1, (msg) =>
        {
            if (msg.Success)
            {

                HeroDomain.Instance.ChangePositionInTeam(heroId1, heroTeamIndex1, heroIndexInTeam1, hero2.id, heroTeamIndex2, heroIndexInTeam2);

                //HeroDomain.Instance.AddHeroToTeam(heroId2, teamIndex1, heroIndexInTeam1);
                //HeroDomain.Instance.RemoveHeroFromTeam(heroId1, teamIndex1);

                //HeroDomain.Instance.AddHeroToTeam(heroId1, teamIndex2, heroIndexInTeam2);
                //HeroDomain.Instance.RemoveHeroFromTeam(heroId2, teamIndex2);

                ShowTeam(_curTeamIndex);
                ShowPage(_pageIndex);
            }
            else
            {
                UIManager.Instance.ShowError(msg.code);
            }

            UIManager.Instance.EndWaiting();
        });
    }

    /// <summary>
    /// 上阵
    /// </summary>
    /// <param name="hero"></param>
    private void OnJoinTeamEventHandler(HeroCardComponent hero)
    {
        if (HeroDomain.Instance.TeamFull(_curTeamIndex))
        {
            UIManager.Instance.ShowMessagebox("队伍已经组满啦~！下次再加入队伍保护本公主吧！");
        }
        else
        {
            UIManager.Instance.BeginWaiting();
            ProCenter.Instance.ReqHeroJoinTeam(hero.DataContent.id, _curTeamIndex + 1, (msg) =>
            {
                UIManager.Instance.EndWaiting();
                if (msg.Success)
                {
                    HeroDomain.Instance.JoinTeam(hero.DataContent.id, _curTeamIndex);
                    ShowTeam(_curTeamIndex);
                    RefreshAllHeroes();
                }
                else
                {
                    UIManager.Instance.ShowError(msg.code);
                }
            });
        }
    }

    /********************** 分页机制  *********************/

    /// <summary>
    /// 下一页
    /// </summary>
    private void NextPage()
    {
        _pageIndex++;
        if (_pageIndex > _totalPage - 1)
        {
            _pageIndex = _totalPage - 1;
            SetPageButtonState();
        }
        else
        {
            ShowPage(_pageIndex);
        }
    }

    /// <summary>
    /// 上一页
    /// </summary>
    private void PrePage()
    {

        _pageIndex--;
        if (_pageIndex < 0)
        {
            _pageIndex = 0;
            SetPageButtonState();
        }
        else
        {
            ShowPage(_pageIndex);
        }

    }

    /// <summary>
    /// 获取某页的英雄
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <returns></returns>
    private List<STUHeroInfo> GetHerosByPage(int pageIndex)
    {
        List<STUHeroInfo> heros = new List<STUHeroInfo>();

        int index = pageIndex * _PER_PAGE_COUNT;
        int count = _PER_PAGE_COUNT;

        //列表里面可能剩余不足显示一页的
        if (_heroList.Count <= index + _PER_PAGE_COUNT)
        {
            count = _heroList.Count - index;
        }

        heros.AddRange(_heroList.GetRange(index, count));

        return heros;
    }


    /// <summary>
    /// 显示某一页
    /// </summary>
    /// <param name="pageIndex"></param>
    private void ShowPage(int pageIndex)
    {
        _pageIndex = pageIndex;

        SetPageLabel();

        SetPageButtonState();

        List<STUHeroInfo> heros = GetHerosByPage(pageIndex);


        foreach (var item in _heroCardAnchor)
        {
            UIUtil.DestoryTransformChild(item);
        }

        for (int i = 0; i < heros.Count; i++)
        {
            GameObject child = UIUtil.AppendChild(_heroCardAnchor[i], _heroCardPrefab);
            HeroCardComponent com = child.GetComponent<HeroCardComponent>();
            com.DataContent = heros[i];
            com.SetState(_selected, _curTeamIndex);
            com.OnJoinTeamEvent = OnJoinTeamEventHandler;
            com.OnChangePositionEvent = OnChangePositionEventHandler;
            com.OnReplaceEvent = OnReplaceEventHandler;
            //com.OnSellEvent = (card) =>
            //{
            //    UIManager.Instance.ShowMessagebox(new MessageBox("遣散这个骑士，你可以获得10个钻石哦~！", MessageBoxButtons.OkCancel, (result) =>
            //    {
            //        if (result == DialogResult.Ok)
            //        {
            //            JsonObject msg = new JsonObject();
            //            string heroId = card.DataContent.id;
            //            msg["heroId"] = heroId;

            //            GChannel.Instance.SendMessage<HeroSellMessage>(msg, (mes) =>
            //            {
            //                if (mes.Success)
            //                {
            //                    HeroDomain.Instance.SellHero(heroId, mes.gold);
            //                    GetComponentInChildren<UIHead>().Refresh();
            //                    RefreshHeros();
            //                    ShowPage(_pageIndex);
            //                }
            //                else
            //                {
            //                    UIManager.Instance.ShowError(mes.code);
            //                }
            //            });
            //        }
            //    }));
            //};
        }
    }
}