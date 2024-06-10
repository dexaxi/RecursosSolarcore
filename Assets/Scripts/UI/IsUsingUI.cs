using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsUsingUI
{
    public static bool IsUsingPopUp = false;
    public static bool IsUsingShop = false;
    public static bool IsUsingDialogue = false;
    public static bool IsInBubblePhase = false;
    public static bool IsInPrephase = false;
    public static bool IsUIEnabled() 
    {
        return IsUsingShop || IsUsingPopUp || IsUsingDialogue || IsInBubblePhase || IsInPrephase;
    }
}
