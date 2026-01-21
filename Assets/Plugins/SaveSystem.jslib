mergeInto(LibraryManager.library, {
    // Функция для скачивания файла
    DownloadFile: function(filenamePtr, dataPtr) {
        var filename = UTF8ToString(filenamePtr);
        var data = UTF8ToString(dataPtr);
        
        var blob = new Blob([data], { type: 'application/json' });
        var url = URL.createObjectURL(blob);
        
        var a = document.createElement('a');
        a.href = url;
        a.download = filename;
        a.style.display = 'none';
        
        document.body.appendChild(a);
        a.click();
        
        setTimeout(function() {
            document.body.removeChild(a);
            URL.revokeObjectURL(url);
        }, 100);
    },
    
    // Функция для вызова загрузки файла
    TriggerFileUpload: function() {
        var input = document.createElement('input');
        input.type = 'file';
        input.accept = '.json';
        input.style.display = 'none';
        
        input.onchange = function(e) {
            var file = e.target.files[0];
            if (file) {
                var reader = new FileReader();
                reader.onload = function(e) {
                    var content = e.target.result;
                    // Вызываем C# функцию с результатом
                    SendMessage('SaveSystem', 'OnFileLoaded', content);
                };
                reader.readAsText(file);
            }
            document.body.removeChild(input);
        };
        
        document.body.appendChild(input);
        input.click();
    },
    
    // Показать сообщение об успехе
    ShowSaveSuccess: function(messagePtr) {
        var message = UTF8ToString(messagePtr);
        alert("✅ " + message);
    },
    
    // Показать сообщение об ошибке
    ShowSaveError: function(messagePtr) {
        var message = UTF8ToString(messagePtr);
        alert("❌ " + message);
    }
});