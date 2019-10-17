﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.
//  site.js will be used in all view maybe




/**
 * 
 * @param {String} _type 
 * @param {String} content 
 * @param {Number} timeout in millisecond
 */
function MakeLayoutAlert(content,timeout=2500,_type="info")
{
    $(`.xbody`).prepend(`<div tabindex="999" class="alert alert-${_type} poya-layout-alert" role="alert">${content}</div>`);
    $(`.poya-layout-alert:last`).focus();
    setTimeout(()=>{
        $(`.poya-layout-alert`).remove();
    },timeout);
}


/**
 * 
 * @param {String} _type 
 * @param {Number} value 1~100
 */
function MakeLayoutProgress(value,_type="info")
{
    if($(`.poya-layout-progress`).length<1)
    {
        $(`.xbody`).prepend(`
            <div class="progress poya-layout-progress" > 
                <div class="progress-bar bg-${_type}  poya-layout-progress-bar" role="progressbar" tabindex="998" style="width: ${value}%" aria-valuenow="${value}" aria-valuemin="0" aria-valuemax="100">
                    ${value}%
                </div>
            </div>
        `);
        $(`.poya-layout-progress-bar:first`).focus();
    }
    else
    {
        $(`.poya-layout-progress-bar`).css(`width`,`${value}%`);
        $(`.poya-layout-progress-bar`).attr(`aria-valuenow`,value);
        $(`.poya-layout-progress-bar`).text(`${value}%`);
    }
    
    if(value==100)
    {
        setTimeout(()=>{
            $(`.poya-layout-progress`).remove();
        },3500);
    }
    
}



(function(){


var THEME_String = "THEME";

var CULTURE_String=`CULTURE`;

var _Language_String=`_Language`;



function PageSize_Input(){
    $("#_pageSize").on("input", function () {
        var _value = $(this).val();
        if (_value.length === 0) return;
        $(this).val(isNaN(_value) ? 8 :  _value>20?20:_value );
    });

}

function GetCulture() {
    var _CULTURE = window.CULTURE;
    if (_CULTURE === undefined) {
        _CULTURE = Cookies.get(this.CULTURE);
        if (_CULTURE === undefined) {
            _CULTURE = "zh-CN";
            Cookies.set(this.CULTURE, _CULTURE);
        }
        window.CULTURE = _CULTURE;
    }
    return String(_CULTURE);
}

function   GetTheme() {
    var _THEME = window.THEME;
    if (_THEME === undefined) {
        _THEME = Cookies.get(this.THEME);
        if (_THEME === undefined) {
            _THEME = "Light";
            Cookies.set(this.THEME, _THEME);
        }
        window.THEME = _THEME;
    }
    return String(_THEME);
}

function   GetThemeInSelectTag() {
    return String($('#Theme  option:selected').val());
}


function ChangeTheme(IsGetValueFromSelectTag = false) {
    var _theme_ = IsGetValueFromSelectTag ? GetThemeInSelectTag() : GetTheme();
    if (IsGetValueFromSelectTag) {
        Cookies.set(THEME_String, _theme_, { expires: 365 });
    }
    location.reload();
}


function  ChangeLanguage() {
    var _Language_=$(`#${_Language_String} option:selected`).val().toString();
    if(_Language_==Cookies.get(CULTURE_String))return;
    Cookies.set(CULTURE_String,`${_Language_}`);
    location.reload()
    return;
}

function KeepLogin() {
    setInterval(function () {
        $.ajax({
            url: "/Home/KeepLogin",
            type: "GET",
            success:()=>{ console.log(`\ud83d\udcaf`);  },
            error:()=>{ console.log(`\u274c`);  }
        });
        
    }, 5 * 60 * 1000);

}


function PageSizeKeyPress() {
    var keyCode = event.keyCode ? event.keyCode : event.which ? event.which : event.charCode;
    if (Number(keyCode) === 13) {
        var _val = Number($(`#_pageSize`).val().replace(/[^0-9]/ig, ""));
        Cookies.set("PageSize",_val<8?8:_val);
        location.reload();
    }
}

/**Generate new guid
 */
function NewGuid() {
    return `${((1 + Math.random()) * 0x10000 | 0).toString(16).substring(1)
        }${((1 + Math.random()) * 0x10000 | 0).toString(16).substring(1)
        }-${((1 + Math.random()) * 0x10000 | 0).toString(16).substring(1)
        }-4${((1 + Math.random()) * 0x10000 | 0).toString(16).substring(1).substr(0, 3)
        }-${((1 + Math.random()) * 0x10000 | 0).toString(16).substring(1)
        }-${((1 + Math.random()) * 0x10000 | 0).toString(16).substring(1)
        }${((1 + Math.random()) * 0x10000 | 0).toString(16).substring(1)
        }${((1 + Math.random()) * 0x10000 | 0).toString(16).substring(1)
        }`.toLowerCase();
}



function ThemeDropdownItemClick()
{
    $(document).on(`click`,`.poya-theme`,(e)=>{ 
        var _poya_theme=$(e.target).attr(`theme`);
        Cookies.set(`THEME`,_poya_theme, { expires: 365 });
        location.reload()
     });
}


$(document).ready(function () {


    $("td,td *,th,th *").css({ "word-wrap": "break-word", "break-word": "break-word" });

    $(`#_Language  option[value='${GetCulture()}']`).attr("selected", true);

    $(document).on("change", "#_Language",()=>{ChangeLanguage();});

    $(document).on("change","#Theme", ()=>{ ChangeTheme(true); });

    $(document).on("click", "#BackA,#BackEle,#go_back",()=>{ history.go(-1);});

    $(document).on(`keypress`,`#_pageSize`, ()=>{ PageSizeKeyPress();   })

    ThemeDropdownItemClick();
    
    PageSize_Input();
    
    KeepLogin();
    
})})();
