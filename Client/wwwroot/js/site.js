window.helperFunctions = {
    closeModal: function (element) {
        $(element).modal('hide');
    },
    getBrowserLocale: function () {
        return (navigator.languages && navigator.languages.length) ? navigator.languages[0] : navigator.userLanguage || navigator.language || navigator.browserLanguage || 'en';
    },
    IFramelyLoad: function () {
        document.querySelectorAll('oembed[url]').forEach(element => {
            iframely.load(element, element.attributes.url.value);
        });
    }
};