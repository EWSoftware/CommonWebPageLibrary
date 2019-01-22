//=============================================================================
// File    : DataChange.js
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : Fri 09/17/2013

var BP_bOnBeforeUnloadFired = false;

// This checks for changed data entry controls on the form.  If called as part of the OnBeforeUnload event, and
// if there are changes, it causes the browser to prompt the user if they want to leave and lose their changes.
// OnSubmit calls this to update the state of the Dirty flag.  It passes a parameter so that it won't attempt to
// set the prompt.  BP_strFormName should be rendered by the page and should contain the form name.
function BP_funCheckForChanges(bInOnSubmit)
{
    var strID, nIdx, nNumOpts, nSkipCnt, nPos, nOptIdx;
    var oElem, oOptions, oOpt, nDefSelIdx, nSelIdx;
    var oForm = document.forms[BP_strFormName];
    var nElemCnt = oForm.elements.length;
    var bPrompt = (typeof(bInOnSubmit) == "undefined");
    var ctlDirty = document.getElementsByName("BP_bIsDirty")[0];

    // This prevents a double prompt that can occur when post back is not cancelled for a hyperlink-type control.
    // Not sure why it happens but this works around it.
    if(bPrompt == true && BP_bOnBeforeUnloadFired == true)
        return;

    // Get current state of Dirty flag
    var bChanged = (ctlDirty.value == "true");

    // IE Only:  The event target is most likely the item that caused the request to leave the page.  If it's in
    // the list of controls that can bypass the check, don't prompt.  The control ID must be an exact match or
    // must end with the name (i.e. it's in a DataGrid).
    strID = "";
    oElem = document.getElementById("__EVENTTARGET");

    if(oElem == null || typeof(oElem) == "undefined" || oElem.value == "")
    {
        // Check the active element if there is no event target
        if(typeof(document.activeElement) != "undefined")
        {
            oElem = document.activeElement;
            strID = oElem.id;
        }
    }
    else
        strID = oElem.value;

    // Some elements may not have an ID but their parent element might so grab that if possible (i.e. AREA
    // elements in a MAP element).
    if(strID == "" && oElem != null && typeof(oElem) != "undefined")
    {
        // Link buttons in DataGrids don't have IDs but do use __doPostBack().  If we see a link with that in its
        // href, assume __doPostBack() is running and skip the check.  The submission will call us again.
        if(oElem.tagName == "A" && oElem.href.indexOf("__doPostBack") != -1)
            return;

        if(typeof(oElem.parentElement) != "undefined")
            strID = oElem.parentElement.id;
    }

    if(strID != "")
    {
        nSkipCnt = BP_arrBypassList.length;

        for(nIdx = 0; nIdx < nSkipCnt; nIdx++)
            if(strID == BP_arrBypassList[nIdx])
                bPrompt = false;
            else
            {
                nPos = strID.length - BP_arrBypassList[nIdx].length;

                if(nPos >= 0)
                    if(strID.substr(nPos) == BP_arrBypassList[nIdx])
                        bPrompt = false;
            }
    }

    // Now we'll figure out if something changed
    nSkipCnt = BP_arrSkipList.length;

    for(nIdx = 0; !bChanged && nIdx < nElemCnt; nIdx++)
    {
        oElem = oForm.elements[nIdx];

        // If the control is in the list of ones to ignore, carry on
        for(nOptIdx = 0; nOptIdx < nSkipCnt; nOptIdx++)
        {
            if(oElem.id == BP_arrSkipList[nOptIdx])
                break;

            nPos = oElem.id.length - BP_arrSkipList[nOptIdx].length;

            if(nPos >= 0)
                if(oElem.id.substr(nPos) == BP_arrSkipList[nOptIdx])
                    break;
        }

        if(nOptIdx < nSkipCnt)
            continue;

        // Check for changes based on the control type
        if(oElem.type == "text" || oElem.tagName == "TEXTAREA")
        {
            if(oElem.value != oElem.defaultValue)
                bChanged = true;
        }
        else
            if(oElem.type == "checkbox" || oElem.type == "radio")
            {
                if(oElem.checked != oElem.defaultChecked)
                    bChanged = true;
            }
            else
                if(oElem.tagName == "SELECT")
                {
                    oOptions = oElem.options;
                    nNumOpts = oOptions.length;
                    nDefSelIdx = nSelIdx = 0;

                    // Search for a change in the default.  If nothing is explicitly marked as the default,
                    // element zero is assumed to have been the default.
                    for(nOptIdx = 0; nOptIdx < nNumOpts; nOptIdx++)
                    {
                        oOpt = oOptions[nOptIdx];

                        if(oOpt.defaultSelected)
                            nDefSelIdx = nOptIdx;

                        if(oOpt.selected)
                            nSelIdx = nOptIdx;
                    }

                    if(nDefSelIdx != nSelIdx)
                        bChanged = true;
                }
    }

    if(bChanged)
    {
        // Pass the dirty state back to the server
        ctlDirty.value = "true";

        // If prompting, set the message
        if(bPrompt)
        {
            event.returnValue = BP_strDataLossMsg;
            BP_bOnBeforeUnloadFired = true;
            window.setTimeout("BP_funClearIfCancelled()", 1000);
        }
    }
}

// This serves two purposes if post-back is cancelled.  It clears the flag that prevents a double prompt and it
// also clears the event target as it doesn't get cleared if you cancel an auto-postback item and then click a
// button for example.
function BP_funClearIfCancelled()
{
    var oElem;

    BP_bOnBeforeUnloadFired = false;

    oElem = document.getElementById("__EVENTTARGET");

    if(oElem != null && typeof(oElem) != "undefined")
        oElem.value = "";
}

// Replace the OnSubmit event.  This is so that we can always update the state of the Dirty flag even when
// controls with AutoPostBack cause the submit.
document.forms[BP_strFormName].BP_RealOnSubmit = document.forms[BP_strFormName].submit;

document.forms[BP_strFormName].submit = function ()
{
    BP_funCheckForChanges(true);

    // It sometimes reports an error if OnBeforeUnload cancels it.  Ignore it.
    try
    {
        document.forms[BP_strFormName].BP_RealOnSubmit();
    }
    catch(e)
    {
    }
};

// IE Only: Hook up the event handler for OnBeforeUnload
window.onbeforeunload = function ()
{
    BP_funCheckForChanges();
};
