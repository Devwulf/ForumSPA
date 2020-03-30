window.helperFunctions = {
    closeModal: function (element) {
        $(element).modal('hide');
    },
    getBrowserLocale: function () {
        return (navigator.languages && navigator.languages.length) ? navigator.languages[0] : navigator.userLanguage || navigator.language || navigator.browserLanguage || 'en';
    }
};