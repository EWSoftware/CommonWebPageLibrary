// #pragma NoCompStart
//=============================================================================
// File    : SetFocus.js
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : Fri 11/26/2004
// #pragma NoCompEnd

// This can be called to set the focus to any control on the form.  If a
// control is embedded in an PageView IE Web Control, the containing PageView
// is selected before giving the control focus.  If focusing a control
// embedded in a data grid, set bFindCtrl to true so that it searches
// for the control by partial name (the data grid modifies the ID to keep
// it unique).  The function returns false to cancel the event if called
// as part of a button or link click event.
function BP_funSetFocus(strID, bFindCtrl)
{
    var nPgIdx, nIdx, nPos, ctl, ctlParent, htmlColl;

    // Do we need to find the control by partial ID?
    if(bFindCtrl == false)
    {
        ctl = document.getElementById(strID);

        // Search for the control if it was found by the NAME attribute
        // rather than by ID (i.e. the ID matched a NAME attribute on a
        // META tag).
        if(ctl != null && typeof(ctl) != "undefined" &&
          (typeof(ctl.id) != "string" || ctl.id != strID))
            bFindCtrl = true;
    }

    if(bFindCtrl == true)
    {
        // True name is unknown.  Find the control ending with
        // the specified name (i.e. it's embedded in a data grid).
        htmlColl = document.getElementsByTagName("*");

        for(nIdx = 0; nIdx < htmlColl.length; nIdx++)
        {
            ctl = htmlColl[nIdx];
            if(typeof(ctl.id) != "undefined")
            {
                nPos = ctl.id.indexOf(strID);
                if(nPos != -1 && ctl.id.substr(nPos) == strID)
                    break;
            }
            else
                ctl = null;
        }
    }

    // If not found, exit
    if(ctl == null || typeof(ctl) == "undefined")
        return false;

    // NOTE: This section is IE-specific.
    // See if there is a parent element.  If so, work back up the chain
    // to see if the control is embedded in an PageView IE Web Control.
    // If so, select that page before giving focus to the control.  If not,
    // it may not work as the control may not be visible.
    if(typeof(ctl.parentElement) != "undefined")
    {
        ctlParent = ctl.parentElement;

        while(ctlParent != null && ctlParent.tagName != "PageView")
            ctlParent = ctlParent.parentElement;

        // If found, set the page as the active one in the containing
        // MultiPage control.
        if(ctlParent != null && ctlParent.tagName == "PageView")
        {
            nPgIdx = ctlParent.PageIndex;
            ctlParent = ctlParent.parentElement;

            if(ctlParent != null && ctlParent.tagName == "MultiPage")
            {
                ctlParent.selectedIndex = nPgIdx;

                // We also have to set the index of any TabStrip
                // associated with the MultiPage.
                htmlColl = document.getElementsByTagName("TabStrip");

                for(nIdx = 0; nIdx < htmlColl.length; nIdx++)
                    if(htmlColl[nIdx].targetID == ctlParent.id)
                    {
                        htmlColl[nIdx].selectedIndex = nPgIdx;
                        break;
                    }
            }
        }
    }
    // End IE-specific section

    // Focus the control.  If it's a table, we may have been asked to
    // set focus to a radio button or checkbox list.  If so, select
    // the control in the first cell of the table.
    if(ctl.tagName == "TABLE")
    {
        ctl = ctl.cells(0);
        ctl = ctl.firstChild;
    }

    ctl.focus();

    // If it is a textbox-type control, select the text in the control
    if(ctl.type == "text" || ctl.tagName == "TEXTAREA")
        ctl.select();

    return false;
}
