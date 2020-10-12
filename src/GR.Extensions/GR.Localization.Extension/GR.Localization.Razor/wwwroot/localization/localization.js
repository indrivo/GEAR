//TODO: Write IndexDb plugin for ui localization


/**
 * Localizer gear plugin
 */
class IndexDbLocalizer {
    /**
     * Constructor
     */
    constructor(indexedDb) {
        this.dbName = "localization";
        this.version = 1;
        this.driver = null;
        this.indexedDb = indexedDb;

        this.init(); //last line
    }

    /**
     * Init
     */
    init() {
        const self = this;
        const dbOpenRequest = this.indexedDb.open(this.dbName, this.version);
        dbOpenRequest.onsuccess = function (event) {
            self.driver = dbOpenRequest.result;
            self.driver.onversionchange = function (e) {
                console.log('Version change triggered, so closing database connection ' +
                    e.oldVersion +
                    ' ' +
                    e.newVersion +
                    ' ' +
                    thisDB);
                self.driver.close();
            };
        }

        const conf = this.getConfiguration();
        const request = this.indexedDb.open(this.dbName, this.version);

        dbOpenRequest.onupgradeneeded = (function (event) {
            const db = event.target.result;
            for (let i = 0; i < conf.length; i++) {
                const tableName = conf[i].name;
                db.createObjectStore(tableName, { keyPath: "ssn" });
            }
        });

        self.storeData();
    }

    storeData() {
        window.loadAsync("/api/LocalizationApi/GetTranslationsForCurrentLanguage").then(response => {
            var transaction = this.driver.transaction(["translations"], "readwrite");
            var objectStore = transaction.objectStore("translations");
            for (let i = 0; i < response.length; i++) {
                objectStore.add({
                    name: i,
                    value: response[i]
                });
            }
        });
    }

    getConfiguration() {
        const tables = [
            {
                name: "translations",
                fields: []
            }
        ];
        return tables;
    }
}


const indexedDB = window.indexedDB || window.mozIndexedDB || window.webkitIndexedDB || window.msIndexedDB;

if (!indexedDB) {
    alert("Sorry!Your browser doesn't support IndexedDB"); // here use local storage
} else {
    window.localizer = new IndexDbLocalizer(indexedDB);
}