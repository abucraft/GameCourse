using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace MemoryTrap
{
    namespace UI
    {
        
        public class AdaptiveByChildren : HorizontalOrVerticalLayoutGroup
        {
            protected float m_preferredWidth;
            protected float m_preferredHeight;
            
            

            public override void CalculateLayoutInputVertical()
            {
                CalcAlongAxis(1, false);
            }

            public void CaculateWidth()
            {
                float left = 0;
                float right = 0;
                for(int i = 0; i < rectTransform.childCount; i++)
                {
                    RectTransform rect = rectTransform.GetChild(i) as RectTransform;
                    
                    float rit = rect.localPosition.x + (1 - rect.pivot.x) * rect.rect.width;
                    float lft = rect.localPosition.x - rect.pivot.x * rect.rect.width;
                    if (left > lft)
                    {
                        left = lft;
                    }
                    if (right<rit)
                    {
                        right = rit;
                    }
                    //Debug.Log(lft);
                    //Debug.Log(rit);
                }
                m_preferredWidth = right - left+padding.left+padding.right;
            }

            public void CaculateHeight()
            {
                float height = 0;
                for (int i = 0; i < rectTransform.childCount; i++)
                {
                    RectTransform rect = rectTransform.GetChild(i) as RectTransform;
                    height += rect.rect.height;
                    //Debug.Log("tp:"+ tp);
                    //Debug.Log("btn:"+ btn);
                }
                m_preferredHeight = height + padding.bottom+padding.top;
            }
            public override void SetLayoutHorizontal()
            {
                CaculateWidth();
                
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_preferredWidth);
                
            }
            public override void SetLayoutVertical()
            {
                CaculateHeight();
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_preferredHeight);
            }
            
        }
    }
}