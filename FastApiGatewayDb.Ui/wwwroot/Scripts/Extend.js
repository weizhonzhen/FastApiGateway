$.extend({
    Checkbox: {
        CheckAll: function (obj, checkboxName) {
            var checkBox = document.getElementsByName(checkboxName);
            for (var i = 0; i < checkBox.length; i++) { checkBox[i].checked = obj.checked; }
        },
        GetCheckedText: function (name) {
            var s = [];
            $("input[name=" + name + "]:checked").each(function () {
                s.push($(this).attr("text"));
            });
            return s.join(",");
        },
        GetCheckedValue: function (name) {
            var s = [];
            $("input[name=" + name + "]:checked").each(function () {
                s.push($(this).val());
            });
            return s.join(",");
        },
        IsCheked: function (name) {
            var isSelect = false;
            $("input[name=" + name + "]:checked").each(function () {
                isSelect = true;
            });
            return isSelect;
        }
    },
    RadioBox: {
        GetValue: function (name) {
            return $('input:radio[name="' + name + '"]:checked').val();
        },
        SetValue: function (name, value) {
            $($("input:radio[name=" + name + "][value=" + value + "]")).prop("checked", true);
        },
        RemoveValue: function (name) {
            $("input:radio[name=" + name + "]").removeAttr('checked'); 
        }
    }
});