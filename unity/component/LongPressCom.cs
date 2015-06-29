using UnityEngine;
using System.Collections;
using System;

public class LongPressCom : MonoBehaviour
{
    private int m_OnPressNum = 0;
    private int m_ShowFrame = PARAM.WAITTIP;
    private bool m_Pressed = false;
    private Action m_OnPressAction = null;
    private Action m_OnUnPressAction = null;

    void Awake()
    {
        UIEventListener.Get(gameObject).onPress += onPress;
    }

    public void RegEvent(Action onpress, Action onunpress)
    {
        m_OnPressAction = onpress;
        m_OnUnPressAction = onunpress;
    }

    private void onPress(GameObject g, bool b)
    {
        m_Pressed = b;
        if (b)
        {
            m_OnPressNum = 0;
        }
        else
        {
            if (m_OnUnPressAction != null)
                m_OnUnPressAction();
        }
    }

    void Update()
    {
        if (m_Pressed)
        {
            m_OnPressNum++;
            if (m_OnPressNum > m_ShowFrame)
            {
                if (m_OnPressAction != null)
                    m_OnPressAction();
            }
        }
    }
}