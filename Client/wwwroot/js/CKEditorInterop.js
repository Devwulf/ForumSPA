window.CKEditorInterop = (() => {
    const editors = {};

    return {
        init(id, dotNetReference) {
            ClassicEditor
                .create(document.getElementById(id), {
                    removePlugins: ['Image', 'ImageCaption', 'ImageStyle', 'ImageToolbar']
                })
                .then(editor => {
                    editors[id] = editor;
                    editor.model.document.on('change:data', () => {
                        let data = editor.getData();

                        const el = document.createElement('div');
                        el.innerHTML = data;
                        if (el.innerText.trim() === '')
                            data = null;

                        dotNetReference.invokeMethodAsync('EditorDataChanged', data);
                    });
                })
                .catch(error => console.error(error));
        },
        destroy(id) {
            editors[id].destroy()
                .then(() => delete editors[id])
                .catch(error => console.log(error));
        },
        setValue(id, inputData) {
            editors[id].data.set(inputData);
        },
        clearValue(id) {
            editors[id].data.set('');
        }
    };
})();