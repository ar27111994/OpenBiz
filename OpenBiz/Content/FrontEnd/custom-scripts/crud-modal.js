$(function () {
  $.ajaxSetup({ cache: false });
  $(document).on("click","a[data-modal='']", function (e) {
    $('#myModalContent').load(this.href, function () {
      $('#myModal').modal({
        keyboard: true
      }, 'show');

      $('select').selectpicker();
      $('input[type="datetime"]').datetimepicker();
      if ($('select').length != 0)
        $('select').parents('form:first').validate().settings.ignore = ':not(select:hidden, input:visible, textarea:visible, textarea:hidden)';
      if ($('textarea').length != 0)
        $('textarea').parents('form:first').validate().settings.ignore = ':not(select:hidden, input:visible, textarea:visible, textarea:hidden)';
      $('select.form-control').hide();
      bindForm(this);
    });
    return false;
  });

  
  $(document).ready(function () {
  $("#grid-search-box").keyup(function () {
    $("#grid-search form").submit();
  });
    $(document).on("click", '#allBox', function () {  //on click 
      $(".chkbx").prop("checked", $("#allBox").is(":checked"));
    });
    
    $(document).ready(function () {
      var target = document.getElementById('web-grid');
      // create an observer instance
      var observer = new MutationObserver(function (mutations) {
        mutations.forEach(function (mutation) {
          var p = $("#grid tfoot > tr > td").clone()    //clone the element
          .children() //select all the children
          .remove()   //remove all the children
          .end()  //again go back to selected element
          .text();//on click
          $('#CurrentPage').val($.trim(p));
        });
      });

      // configuration of the observer:
      var config = { attributes: true, childList: true, characterData: true };

      // pass in the target node, as well as the observer options
      observer.observe(target, config);
      });
    
      $(document).on("change", '.chkbx', function () {  //on change
      var vis = "none";
      $('.chkbx').each(function () {
        if ($(this).is(":checked")) {
          vis = "block";
          return;
        }
      });
      document.getElementById("del").style.display = vis;
    });
  });
  $(document).on("click", '#del', function () {
    var array = [];
    $('#myModalContent').html('<div class="modal-body">Are you sure you want to delete the selected items?</div><div class="modal-footer"><span id="progress" class="text-center" style="display: none;"><img src="../../Content/Images/wait.gif" alt="wiat" />Wait..</span><button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button><a id="cnfrm" class="btn btn-danger btn-ok">Delete</a></div>');
      $('#myModal').modal({
        keyboard: true
      }, 'show');
      $('#cnfrm').click(function () {
        $('#progress').show();
        $('.ids').each(function () {
          if ($(this).is(":checked")) {
            array.push($(this).val());
          }
        });
        $.ajax({
          url: $('#del').attr("href"),
          type: "POST",
          data: { ids: array },
          success: function (result) {
            if (result.success) {
              document.getElementById("del").style.display = "none";
              $('#myModal').modal('hide');
              reloadGrid();
            } else {
              $('#myModalContent').html('<div class="modal-body text-danger">Deletion Failed!</div><div class="modal-footer"><button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button></div>');
              $('#myModal').modal({
                keyboard: true
              }, 'show');
            }
          }
        });
      });
    return false;
  });
//  $('#export').submit(function () {
//    $('#progress').show();
//    $.ajax({
//      url: this.action,
//      type: this.method,
//      data: $(this).serialize(),
//      success: function (result) {
//        if (result.success) {
//          $('#myModal').modal('hide');
//          $('#progress').hide();
//        } else {
//          $('#progress').hide();
//          //$('#myModalContent').html(result);
//        }
//      }
//    });
//    return false;
//  });
});

function bindForm(dialog) {
//$("form",dialog).data("validator").settings.submitHandler = function (form) { 
  $('form', dialog).submit(function(event) {
    event.preventDefault();
    for (instance in CKEDITOR.instances) {
      CKEDITOR.instances[instance].updateElement();
    }
    if ($(this).valid()) {
      var formdata = false;
      
      formdata = new FormData($(this)[0]);
      if ($('input[type=file]').length>0) {
        formdata.append("upload", $('input[type=file]')[0].files[0])
      }
      $('#progress').show();
      $.ajax({
        url: this.action,
        type: this.method,
        data: formdata ? formdata : $(this).serialize(),
        dataType: 'json',
        contentType: false,
        processData: false,
        success: function (result) {
          if (result.success) {
            $('#myModal').modal('hide');
            $('#progress').hide();
            reloadGrid();
          } else {
            $('#progress').hide();
            //$('#myModalContent').html(result);
            bindForm();
          }
        }
      });
      return false;
    }
    
  });
  //};
}