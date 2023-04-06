ThemeBuilder.__badges_group = function() { return {
"id": "badges", 
"widgets": [
    { 
	    "name": "dxList",
	    "supportedThemes": ["all"],
	    "options": { 
		    "dataSource": [
			    { "text": "first", "badge": "127" },
			    { "text": "second" }
		    ]
	    }
    },
    { 
	    "name": "dxNavBar",
	    "supportedThemes": ["all"],
	    "options": { 
		    "dataSource": [
			    { "text": "user", "icon": "user" },
			    { "text": "find", "icon": "find" },
			    { "text": "favorites", "icon": "favorites" },
			    { "text": "about", "icon": "info" },
			    { "text": "home", "icon": "home", "badge": 77 },
			    { "text": "URI", "icon": "tips", "disabled": true }
		    ]
	    }
    }
]
};};
ThemeBuilder.__base_common_group = function() { return {
    "id": "base.common",
    "useFieldsetForThemes": [ "ios7-default", "android5-light" ],
    "widgets": [
        {
            "name": "dxDataGrid",
            "supportedThemes": [ "generic-dark", "generic-light", "generic-carmine", "generic-darkmoon", "generic-softblue", "generic-darkviolet", "generic-greenmist", "generic-carmine-compact", "generic-darkmoon-compact", "generic-softblue-compact", "generic-darkviolet-compact", "generic-greenmist-compact", "generic-light-compact", "generic-dark-compact" ],
            "options": {
                "rowAlternationEnabled": true,
                "dataSource": {
                    "asyncLoadEnabled": false,
                    "store": {
                        "data": [
                            {
                                "CustomerID": "VINET",
                                "Freight": "32.3800",
                                "ShipCountry": "France",
                                "ShipName": "Vins et alcools Chevalier"
                            },
                            {
                                "CustomerID": "TOMSP",
                                "Freight": "11.6100",
                                "ShipCountry": "Germany",
                                "ShipName": "Toms Spezialitaten"
                            },
                            {
                                "CustomerID": "HANAR1",
                                "Freight": "65.8300",
                                "ShipCountry": "Brazil",
                                "ShipName": "Hanari Carnes"
                            },
                            {
                                "CustomerID": "VICTE",
                                "Freight": "41.3400",
                                "ShipCountry": "France",
                                "ShipName": "Richter Supermarkt"
                            },
                            {
                                "CustomerID": "SUPRD",
                                "Freight": "51.3000",
                                "ShipCountry": "Belgium",
                                "ShipName": "Victuailles en stock"
                            },
                            {
                                "CustomerID": "HANAR",
                                "Freight": "58.1700",
                                "ShipCountry": "Brazil",
                                "ShipName": "Supremes delices"
                            }
                        ],
                        "type": "array",
                        "key": "CustomerID"
                    }
                },
                "selectedRowKeys": [ "TOMSP", "SUPRD" ],
                "height": "230px",
                "selection": {
                    "mode": "multiple"
                },
				"pager": {
                    "showPageSizeSelector": true
                },
				"paging": {
                    "pageSize": 4
                },
                "hoverStateEnabled": true,
                "allowColumnResizing": true,
                "allowColumnReordering": true,
                "columns": [
                    "ShipName",
                    "Freight",
                    "ShipCountry"
                ]
            }
        },
		{
            "name": "dxList",
            "supportedThemes": ["generic-dark", "generic-light", "generic-carmine", "generic-darkmoon", "generic-softblue", "generic-darkviolet", "generic-greenmist", "generic-carmine-compact", "generic-darkmoon-compact", "generic-softblue-compact", "generic-darkviolet-compact", "generic-greenmist-compact", "generic-light-compact", "generic-dark-compact"],
            "options": {
                "items": [
                        {
                            key: "Ship Country: Germany",
                            items: [
                                { "text": "Toms Spezialitaten" },
                                { "text": "Ottilies Kaseladen", "badge": 5, "showChevron": true }
                            ]
                        },
                        {
                            key: "Ship Country: France",
                            items: [
                                { "text": "Vins et alcools Chevalier" },
                                { "text": "Richter Supermarkt" },
                                { "text": "Blondel pere et fils" },
								{ "text": "Que Delicia", "badge": 33 },
                                { "text": "Folk och fa HB", "showChevron": true }
                            ]
                        },
                        {
                            key: "Ship Country: Switzerland",
                            items: [
                                { text: "Hanari Carnes" },
                                { text: "Chop-suey Chinese" }
                            ]
                        }
                ],
                "grouped": true,
                "height": "262px"
            }
        },
        {
            "name": "dxSelectBox",
            "supportedThemes": [ "generic-dark", "generic-light", "generic-carmine", "generic-darkmoon", "generic-softblue", "generic-darkviolet", "generic-greenmist", "generic-carmine-compact", "generic-darkmoon-compact", "generic-softblue-compact", "generic-darkviolet-compact", "generic-greenmist-compact", "generic-light-compact", "generic-dark-compact" ],
            "options": {
                "items": [ "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" ]
            }
        },
        {
            "name": "dxSelectBox",
            "supportedThemes": [ "generic-dark", "generic-light", "generic-carmine", "generic-darkmoon", "generic-softblue", "generic-darkviolet", "generic-greenmist", "generic-carmine-compact", "generic-darkmoon-compact", "generic-softblue-compact", "generic-darkviolet-compact", "generic-greenmist-compact", "generic-light-compact", "generic-dark-compact" ],
            "options": {
                "items": [ "Bob", "John", "Frank", "Robert", "Paul", "Elizabeth", "Mary" ]
            }
        },
        {
            "name": "dxSlider",
            "supportedThemes": [ "generic-dark", "generic-light", "generic-carmine", "generic-darkmoon", "generic-softblue", "generic-darkviolet", "generic-greenmist", "generic-carmine-compact", "generic-darkmoon-compact", "generic-softblue-compact", "generic-darkviolet-compact", "generic-greenmist-compact", "generic-light-compact", "generic-dark-compact" ],
            "options": {
                "min": 0,
                "max": 100,
                "value": 38
            }
        },
        {
            "name": "dxLookup",
            "supportedThemes": [ "generic-dark", "generic-light", "generic-carmine", "generic-darkmoon", "generic-softblue", "generic-darkviolet", "generic-greenmist", "generic-carmine-compact", "generic-darkmoon-compact", "generic-softblue-compact", "generic-darkviolet-compact", "generic-greenmist-compact", "generic-light-compact", "generic-dark-compact" ],
            "options": {
                "items": [
                    "New York",
                    "Los Angeles",
                    "Chicago",
                    "Houston",
                    "Philadelphia",
                    "Phoenix",
                    "San Antonio",
                    "San Diego",
                    "Dallas",
                    "San Jose",
                    "Jacksonville",
                    "Indianapolis",
                    "Austin",
                    "San Francisco",
                    "Columbus",
                    "Fort Worth",
                    "Charlotte",
                    "Detroit",
                    "El Paso",
                    "Memphis",
                    "Boston",
                    "Seattle",
                    "Denver",
                    "Baltimore",
                    "Washington",
                    "Nashville",
                    "Louisville",
                    "Milwaukee",
                    "Portland",
                    "Oklahoma"
                ],
                "popupHeight": 500
            }
        },
		{
            "name": "dxButton",
            "supportedThemes": [ "generic-dark", "generic-light", "generic-carmine", "generic-darkmoon", "generic-softblue", "generic-darkviolet", "generic-greenmist", "generic-carmine-compact", "generic-darkmoon-compact", "generic-softblue-compact", "generic-darkviolet-compact", "generic-greenmist-compact", "generic-light-compact", "generic-dark-compact" ],
            "options": {
                "text": "Danger",
                "type": "danger"
            }
        },
		{
            "name": "dxButton",
            "supportedThemes": ["generic-dark", "generic-light", "generic-carmine", "generic-darkmoon", "generic-softblue", "generic-darkviolet", "generic-greenmist", "generic-carmine-compact", "generic-darkmoon-compact", "generic-softblue-compact", "generic-darkviolet-compact", "generic-greenmist-compact", "generic-light-compact", "generic-dark-compact"],
            "options": {
                "text": "Success",
                "type": "success"
            }
        },
		{
            "name": "dxButton",
            "supportedThemes": [ "generic-dark", "generic-light", "generic-carmine", "generic-darkmoon", "generic-softblue", "generic-darkviolet", "generic-greenmist", "generic-carmine-compact", "generic-darkmoon-compact", "generic-softblue-compact", "generic-darkviolet-compact", "generic-greenmist-compact", "generic-light-compact", "generic-dark-compact" ],
            "options": {
                "text": "Default",
                "type": "default"
            }
        },
        {
             "name": "dxButton",
             "supportedThemes": ["generic-dark", "generic-light", "generic-carmine", "generic-darkmoon", "generic-softblue", "generic-darkviolet", "generic-greenmist", "generic-carmine-compact", "generic-darkmoon-compact", "generic-softblue-compact", "generic-darkviolet-compact", "generic-greenmist-compact", "generic-light-compact", "generic-dark-compact"],
             "options": {
                 "text": "Normal",
                 "type": "normal"
             }
        },

        {
            "name": "dxTextBox",
            "title": "dxTextBox",
            "supportedThemes": [ "ios7-default", "android5-light" ],
            "options": {
                "value": "Textbox widget"
            }
        },
        {
            "name": "dxSelectBox",
            "title": "dxSelectBox",
            "supportedThemes": [ "ios7-default", "android5-light" ],
            "options": {
                "items": [ "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" ]
            }
        },
        {
            "name": "dxLookup",
            "title": "dxLookup",
            "supportedThemes": [ "ios7-default", "android5-light" ],
            "options": {
                "items": [
                    "New York",
                    "Los Angeles",
                    "Chicago",
                    "Houston",
                    "Philadelphia",
                    "Phoenix",
                    "San Antonio",
                    "San Diego",
                    "Dallas",
                    "San Jose",
                    "Jacksonville",
                    "Indianapolis",
                    "Austin",
                    "San Francisco",
                    "Columbus",
                    "Fort Worth",
                    "Charlotte",
                    "Detroit",
                    "El Paso",
                    "Memphis",
                    "Boston",
                    "Seattle",
                    "Denver",
                    "Baltimore",
                    "Washington",
                    "Nashville",
                    "Louisville",
                    "Milwaukee",
                    "Portland",
                    "Oklahoma"
                ],
                "popupHeight": 500
            }
        },
        {
            "name": "dxCheckBox",
            "title": "dxCheckBox",
            "supportedThemes": [ "ios7-default", "android5-light" ],
            "options": {
                "checked": true
            }
        },
        {
            "name": "dxSwitch",
            "title": "dxSwitch",
            "supportedThemes": [ "ios7-default", "android5-light" ],
            "options": {
                "value": true
            }
        },
        {
            "name": "dxSwitch",
            "title": "Disabled dxSwitch",
            "supportedThemes": [ "ios7-default", "android5-light" ],
            "options": {
                "disabled": true,
                "value": true
            }
        },
        {
            "name": "dxButton",
            "title": "dxButton",
            "supportedThemes": [ "ios7-default" ],
            "options": {
                "text": "Normal Button",
                "type": "normal"
            }
        },
		{
            "name": "dxScheduler",
            "supportedThemes": [ "generic-dark", "generic-light", "generic-carmine", "generic-darkmoon", "generic-softblue", "generic-darkviolet", "generic-greenmist", "generic-carmine-compact", "generic-darkmoon-compact", "generic-softblue-compact", "generic-darkviolet-compact", "generic-greenmist-compact", "generic-light-compact", "generic-dark-compact" ],
            "options": {
                "dataSource": [
                    {
                        text: "Website Re-Design Plan",
                        ownerId: [4],
                        startDate: new Date(2015, 4, 25, 9, 30),
                        endDate: new Date(2015, 4, 25, 11, 30)
                    }, {
                        text: "Book Flights to San Fran for Sales Trip",
                        ownerId: [2],
                        startDate: new Date(2015, 4, 25, 12, 0),
                        endDate: new Date(2015, 4, 25, 13, 0)
                    }, {
                        text: "Install New Router in Dev Room",
                        ownerId: [1],
                        startDate: new Date(2015, 4, 25, 14, 30),
                        endDate: new Date(2015, 4, 25, 15, 30)
                    }, {
                        text: "Approve Personal Computer Upgrade Plan",
                        ownerId: [3],
                        startDate: new Date(2015, 4, 26, 10, 0),
                        endDate: new Date(2015, 4, 26, 11, 0)
                    }, {
                        text: "Final Budget Review",
                        ownerId: [1],
                        startDate: new Date(2015, 4, 26, 12, 0),
                        endDate: new Date(2015, 4, 26, 13, 35)
                    }, {
                        text: "New Brochures",
                        ownerId: [4],
                        startDate: new Date(2015, 4, 26, 14, 30),
                        endDate: new Date(2015, 4, 26, 15, 45)
                    }, {
                        text: "Install New Database",
                        ownerId: [2],
                        startDate: new Date(2015, 4, 27, 9, 45),
                        endDate: new Date(2015, 4, 27, 11, 15)
                    }, {
                        text: "Approve New Online Marketing Strategy",
                        ownerId: [3, 4],
                        startDate: new Date(2015, 4, 27, 12, 0),
                        endDate: new Date(2015, 4, 27, 14, 0)
                    }, {
                        text: "Upgrade Personal Computers",
                        ownerId: [2],
                        startDate: new Date(2015, 4, 27, 15, 15),
                        endDate: new Date(2015, 4, 27, 16, 30)
                    }, {
                        text: "Prepare 2015 Marketing Plan",
                        ownerId: [1, 3],
                        startDate: new Date(2015, 4, 28, 11, 0),
                        endDate: new Date(2015, 4, 28, 13, 30)
                    }, {
                        text: "Brochure Design Review",
                        ownerId: [4],
                        startDate: new Date(2015, 4, 28, 14, 0),
                        endDate: new Date(2015, 4, 28, 15, 30)
                    }, {
                        text: "Create Icons for Website",
                        ownerId: [3],
                        startDate: new Date(2015, 4, 29, 10, 0),
                        endDate: new Date(2015, 4, 29, 11, 30)
                    }, {
                        text: "Upgrade Server Hardware",
                        ownerId: [4],
                        startDate: new Date(2015, 4, 29, 14, 30),
                        endDate: new Date(2015, 4, 29, 16, 0)
                    }, {
                        text: "Submit New Website Design",
                        ownerId: [1],
                        startDate: new Date(2015, 4, 29, 16, 30),
                        endDate: new Date(2015, 4, 29, 18, 0)
                    }, {
                        text: "Launch New Website",
                        ownerId: [2],
                        startDate: new Date(2015, 4, 29, 12, 20),
                        endDate: new Date(2015, 4, 29, 14, 0)
                    }, {
                        text: "Stand-up meeting",
                        ownerId: [1, 2, 3, 4],
                        startDate: new Date(2015, 4, 25, 9, 0),
                        endDate: new Date(2015, 4, 25, 9, 15),
                        recurrenceRule: "FREQ=DAILY;BYDAY=MO,TU,WE,TH,FR;UNTIL=20150530"
                    }],
                "views": ["week", "workWeek", "day"],
                "currentView": "workWeek",
                "currentDate": new Date(2015, 4, 25),
                "useDropDownViewSwitcher": false,
                "firstDayOfWeek": 0,
                "startDayHour": 9,
                "endDayHour": 19,
                "resources": [{
                    "field": "ownerId",
                    "allowMultiple": true,
                    "dataSource": [
                        {
                            text: "Samantha Bright",
                            id: 1
                        }, {
                            text: "John Heart",
                            id: 2
                        }, {
                            text: "Todd Hoffman",
                            id: 3
                        }, {
                            text: "Sandra Johnson",
                            id: 4
                        }
                    ],
                    "label": "Owner"
                }],
                "width": "100%",
                "height": 400
            }
        }
    ]
};};
ThemeBuilder.__base_typography_group = function() { return {
"id": "base.typography", 
"widgets": [
    {
        "name": "dxScrollView",
        "supportedThemes": [ "generic-dark", "generic-light", "generic-carmine", "generic-darkmoon", "generic-softblue", "generic-darkviolet", "generic-greenmist", "generic-carmine-compact", "generic-darkmoon-compact", "generic-softblue-compact", "generic-darkviolet-compact", "generic-greenmist-compact", "generic-light-compact", "generic-dark-compact" ],
        "prerenderAction": "LOAD_GENERIC_TYPOGRAPHY_CONTENT",
        "options": { }
    },
    {
		"name": "dxScrollView",
		"supportedThemes": ["android5-light"],
		"prerenderAction": "LOAD_ANDROID5_TYPOGRAPHY_CONTENT",
		"options": { }
	},
    {
		"name": "dxScrollView",
		"supportedThemes": ["ios7-default"],
		"prerenderAction": "LOAD_IOS7_TYPOGRAPHY_CONTENT",
		"options": { }
	}
]
};};
ThemeBuilder.__buttons_danger_group = function() { return {
"id": "buttons.danger", 
"widgets": [
    {
        "name": "dxButton",
        "supportedThemes": [ "generic-dark", "generic-light", "generic-carmine", "generic-darkmoon", "generic-softblue", "generic-darkviolet", "generic-greenmist", "generic-carmine-compact", "generic-darkmoon-compact", "generic-softblue-compact", "generic-darkviolet-compact", "generic-greenmist-compact", "generic-light-compact", "generic-dark-compact", "android5-light" ],
        "options": {
            "text": "Danger",
            "type": "danger"
        }
    },
    {
		"name": "dxButton",
		"supportedThemes": ["ios7-default"],
		"options": {
			"text": "Danger",
            "type": "danger",
            "icon": "clear" 
		}
	}
]
};};
ThemeBuilder.__buttons_default_group = function() { return {
"id": "buttons.default", 
"widgets": [
    {
        "name": "dxButton",
        "supportedThemes": [ "generic-dark", "generic-light", "generic-carmine", "generic-darkmoon", "generic-softblue", "generic-darkviolet", "generic-greenmist", "generic-carmine-compact", "generic-darkmoon-compact", "generic-softblue-compact", "generic-darkviolet-compact", "generic-greenmist-compact", "generic-light-compact", "generic-dark-compact", "android5-light" ],
        "options": {
            "text": "Default",
            "type": "default"
        }
    },
    {
		"name": "dxButton",
		"supportedThemes": ["ios7-default"],
		"options": {
			"text": "Default",
            "type": "default",
            "icon": "info"
		}
	}
]
};};
ThemeBuilder.__buttons_flat_group = function() { return {
    "id": "buttons.flat",
    "useFieldsetForThemes": [ "android5-light" ],
    "widgets": [
        {
            "name": "dxButton",
            "title": "Flat button",
			"supportedThemes": ["android5-light"],
			"options": {
				"text": "Button",
				"type": "normal"
			}
		}
    ]
};};
ThemeBuilder.__buttons_normal_group = function() { return {
"id": "buttons.normal", 
"widgets": [
	{
			"name": "dxButton",
			"supportedThemes": ["all"],
			"options": {
				"text": "Normal",
				"type": "normal"
			}
		}
]
};};
ThemeBuilder.__buttons_success_group = function() { return {
"id": "buttons.success", 
"widgets": [
    {
        "name": "dxButton",
        "supportedThemes": [ "generic-dark", "generic-light", "generic-carmine", "generic-darkmoon", "generic-softblue", "generic-darkviolet", "generic-greenmist", "generic-carmine-compact", "generic-darkmoon-compact", "generic-softblue-compact", "generic-darkviolet-compact", "generic-greenmist-compact", "generic-light-compact", "generic-dark-compact", "android5-light" ],
        "options": {
            "text": "Success",
            "type": "success"
        }
    },

    {
		"name": "dxButton",
		"supportedThemes": ["ios7-default"],
		"options": {
			"text": "Success",
            "type": "success",
            "icon":  "gift"
		}
    }
]
};};
ThemeBuilder.__datagrid_group = function() { return {
    "id": "datagrid",
    "ord": 4,
    "widgets": [
        {
            "name": "dxDataGrid",
            "supportedThemes": [ "all" ],
            "options": {
                "columnChooser": { "enabled": true },
                "rowAlternationEnabled": true,
                "dataSource": {
                    "store": [
                        {
                            "CustomerID": "VINET",
                            "OrderDate": "1996/07/04",
                            "Freight": "32.3800",
                            "ShipName": "Vins et alcools Chevalier",
                            "ShipCity": "Reims",
                            "ShipCountry": "France"
                        },
                        {
                            "CustomerID": "TOMSP",
                            "OrderDate": "1996/07/05",
                            "Freight": "11.6100",
                            "ShipName": "Toms Spezialitaten",
                            "ShipCity": "Munster",
                            "ShipCountry": "Germany"
                        },
                        {
                            "CustomerID": "HANAR",
                            "OrderDate": "1996/07/08",
                            "Freight": "65.8300",
                            "ShipName": "Hanari Carnes",
                            "ShipCity": "Rio de Janeiro",
                            "ShipCountry": "Brazil"
                        },
                        {
                            "CustomerID": "VICTE",
                            "OrderDate": "1996/07/08",
                            "Freight": "41.3400",
                            "ShipName": "Victuailles en stock",
                            "ShipCity": "Lyon",
                            "ShipCountry": "France"
                        },
                        {
                            "CustomerID": "SUPRD",
                            "OrderDate": "1996/07/09",
                            "Freight": "51.3000",
                            "ShipName": "Supremes delices",
                            "ShipCity": "Charleroi",
                            "ShipCountry": "Belgium"
                        },
                        {
                            "CustomerID": "HANAR",
                            "OrderDate": "1996/07/10",
                            "Freight": "58.1700",
                            "ShipName": "Hanari Carnes",
                            "ShipCity": "Rio de Janeiro",
                            "ShipCountry": "Brazil"
                        },
                        {
                            "CustomerID": "CHOPS",
                            "OrderDate": "1996/07/11",
                            "Freight": "22.9800",
                            "ShipName": "Chop-suey Chinese",
                            "ShipCity": "Bern",
                            "ShipCountry": "Switzerland"
                        },
                        {
                            "CustomerID": "RICSU",
                            "OrderDate": "1996/07/12",
                            "Freight": "148.3300",
                            "ShipName": "Richter Supermarkt",
                            "ShipCity": "Geneve",
                            "ShipCountry": "Switzerland"
                        },
                        {
                            "CustomerID": "WELLI",
                            "OrderDate": "1996/07/15",
                            "Freight": "13.9700",
                            "ShipName": "Wellington Importadora",
                            "ShipCity": "Resende",
                            "ShipCountry": "Brazil"
                        },
                        {
                            "CustomerID": "HILAA",
                            "OrderDate": "1996/07/16",
                            "Freight": "81.9100",
                            "ShipName": "HILARION-Abastos",
                            "ShipCity": "San Cristobal",
                            "ShipCountry": "Venezuela"
                        },
                        {
                            "CustomerID": "ERNSH",
                            "OrderDate": "1996/07/17",
                            "Freight": "140.5100",
                            "ShipName": "Ernst Handel",
                            "ShipCity": "Graz",
                            "ShipCountry": "Austria"
                        }
                    ]
                },
                "filterRow": {
                    "visible": true
                },
                "height": "500px",
                "paging": {
                    "pageSize": 10
                },
                "pager": {
                    "showPageSizeSelector": true
                },
                "searchPanel": {
                    "visible": true
                },
                "groupPanel": {
                    "visible": true
                },
                "selection": {
                    "mode": "multiple"
                },
                "hoverStateEnabled": true,
                "allowColumnResizing": true,
                "allowColumnReordering": true,
                "editing": {
                    "allowUpdating": true,
                    "allowDeleting": true,
                    "mode": "batch"
                },
                "columns": [
                    {
                        "dataField": "CustomerID",
                        "allowFiltering": false
                    },
                    {
                        "dataField": "OrderDate",
                        "dataType": "date"
                    },
                    "ShipName",
                    {
                        "dataField": "ShipCity",
                        "groupIndex": 0,
                        "filterOperations": [ "startswith", "contains", "=" ]
                    },
                    {
                        "dataField": "ShipCountry",
                        "filterOperations": false,
                        "selectedFilterOperation": "startswith"
                    },
                    "Freight"
                ],
                "summary": {
                    "totalItems": [
                        {
                            "column": "Freight",
                            "summaryType": "sum",
                            "displayFormat": "Total: {0}"
                        }
                    ],
                    "groupItems": [
                        {
                            "column": "CustomerID",
                            "summaryType": "count"
                        }
                    ]
                }
            }
        }
    ]
};};
ThemeBuilder.__editors_autocomplete_group = function() { return {
"id": "editors.autocomplete",
"ord": 3,
"widgets": [
		{ 
			"name": "dxAutocomplete",
			"supportedThemes": ["all"],
			"options": {
				"items": ["New York", "Los Angeles", "Chicago", "Houston", "Philadelphia", "Phoenix", "San Antonio",
						"San Diego", "Dallas", "San Jose", "Jacksonville", "Indianapolis", "Austin", "San Francisco",
						"Columbus", "Fort Worth", "Charlotte", "Detroit", "El Paso", "Memphis", "Boston", "Seattle",
						"Denver", "Baltimore", "Washington", "Nashville", "Louisville", "Milwaukee", "Portland", "Oklahoma"],
				"placeholder": "Enter city name"
			}
		}
]
};};
ThemeBuilder.__editors_calendar_group = function() { return {
"id": "editors.calendar", 
"widgets": [
		{
			"name": "dxCalendar",
			"supportedThemes": ["all"],
			"options": { }
		}
]
};};
ThemeBuilder.__editors_checkbox_group = function() { return {
"id": "editors.checkbox",
"ord": 4,
"widgets": [
	{
			"name": "dxCheckBox",
			"title": "dxCheckBox",
			"supportedThemes": ["all"],
			"options": {
				"checked": true
            }
		},
		{
			"name": "dxCheckBox",
			"title": "Disabled dxCheckBox",
			"supportedThemes": ["all"],
			"options": {
				"disabled": true
            }
		}
]
};};
ThemeBuilder.__editors_colorbox_group = function() { return {
"id": "editors.colorbox", 
"widgets": [
		{
			"name": "dxColorBox",
			"supportedThemes": ["all"],
			"options": {
				"value": "rgba(89, 168, 61, .7)",
				"editAlphaChannel": true
			}
		}
]
};};
ThemeBuilder.__editors_fileuploader_group = function() { return {
    "id": "editors.fileuploader",
    "widgets": [
        {
            "name": "dxFileUploader",
            "supportedThemes": [ "all" ],
            "options": { }
        }
    ]
};};
ThemeBuilder.__editors_lookup_group = function() { return {
"id": "editors.lookup", 
"ord": 6,
"widgets": [
{  
			"name": "dxLookup",
			"supportedThemes": ["all"],
			"options": {
				"items": ["New York", "Los Angeles", "Chicago", "Houston", "Philadelphia", "Phoenix", "San Antonio",
				"San Diego", "Dallas", "San Jose", "Jacksonville", "Indianapolis", "Austin", "San Francisco",
				"Columbus", "Fort Worth", "Charlotte", "Detroit", "El Paso", "Memphis", "Boston", "Seattle",
				"Denver", "Baltimore", "Washington", "Nashville", "Louisville", "Milwaukee", "Portland", "Oklahoma"],
				"popupHeight": 500,
				"usePopover": false
			}
		}
]
};};
ThemeBuilder.__editors_numberbox_group = function() { return {
"id": "editors.numberbox", 
"widgets": [
		{
			"name": "dxNumberBox",
			"supportedThemes": ["all"],
			"options": {
				"showSpinButtons": true
            }
		}
]
};};
ThemeBuilder.__editors_radiogroup_group = function() { return {
"id": "editors.radiogroup", 
"ord": 7,
"widgets": [
{
			"name": "dxRadioGroup",
			"supportedThemes": ["all"],
			"options": {
				"items": ["Tea", "Coffee", "Juice"],
				"selectedIndex": 0
			}
		}
]
};};
ThemeBuilder.__editors_selectbox_group = function() { return {
"id": "editors.selectbox", 
"widgets": [
		{
			"name": "dxSelectBox",
			"title": "dxSelectBox",
			"supportedThemes": ["all"],
			"options": {
				"items": ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"]
			}
		}
]
};};
ThemeBuilder.__editors_switch_group = function() { return {
"id": "editors.switch", 
"ord": 8,
"widgets": [
{
			"name": "dxSwitch",
			"title": "dxSwitch",
			"supportedThemes": ["all"],
			"options": { }
		},
		{
			"name": "dxSwitch",
			"title": "Disabled dxSwitch",
			"supportedThemes": ["all"],
			"options": { 
				"disabled": true,
				"value": true
			}
		}
]
};};
ThemeBuilder.__editors_tagbox_group = function() { return {
    "id": "editors.tagbox",
    "widgets": [
        {
            "name": "dxTagBox",
            "supportedThemes": ["all"],
            "options": {
                "items": ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"]
            }
        }
    ]
};};
ThemeBuilder.__editors_texteditors_group = function() { return {
	"id": "editors.texteditors", 
	"ord": 9,
	"widgets": [
		{
			"name": "dxTextBox",
			"title": "dxTextBox",
			"supportedThemes": ["all"],
			"options": {
				"placeholder": "Text box widget",
				"mode": "search"
            }
		},
		{
			"name": "dxTextArea",
			"title": "dxTextArea",
			"supportedThemes": ["all"],
			"options": {
				"value": "Multiline text box widget"
            }
		}
	]
};};
ThemeBuilder.__editors_validation_group = function() { return {
    "id": "editors.validation",
    "widgets": [
        {

            "name": "dxTextBox",
            "title": "Name",
            "supportedThemes": [ "all" ],
            "afterRenderAction": "VALIDATION_ACTION",
            "options": {
                "placeholder": "Name",
                "validationOptions": {
                    "name": "Name",
                    "validationRules": [
                        {
                            "type": "required",
                            "message":  ""
                        }
                    ]
                }
            }
        },
        {

            "name": "dxTextBox",
            "title": "Password",
            "supportedThemes": [ "all" ],
            "afterRenderAction": "VALIDATION_ACTION",
            "options": {
                "placeholder": "Password",
                "mode":  "password",
                "validationOptions": {
                    "name": "Password",
                    "validationRules": [
                        {
                            "type": "required",
                            "message":  ""
                        }
                    ]
                }
            }
        }
    ]
};};
ThemeBuilder.__filterbuilder_group = function() { return {
    "id": "filterbuilder", 
    "widgets": [{
        "name": "dxFilterBuilder",
        "supportedThemes": [ "all" ],
        "options": {
            "value":[
                ["Name", "=", "Projector PlusHD"],
                "or",
                [
                    ["Category", "=", "Monitors"],
                    ["Price", "<", "1300"]
                ],
                "or",
                [
                    ["Category", "=", "Televisions"], 
                    ["Price", "<", "4000"]      
                ]
            ],
            "fields": [{
	            "dataField": "ID",
	            "dataType": "number"
	        }, {
	            "dataField": "Name"
	        }, {
	            "dataField": "Price",
	            "dataType": "number"
	        }, {
	            "dataField": "Current_Inventory",
	            "dataType": "number"
	        }, {
	            "dataField": "Category"
	        }]
        }
    }]
};};
ThemeBuilder.__form_group = function() { return {
    "id": "form",
    "widgets": [
        {
            "name": "dxForm",
            "supportedThemes": [ "all" ],
            "options": {
                "labelLocation": "top",
                "formData": {
                    "Address": "351 S Hill St.",
                    "City": "Los Angeles",
                    "State": "CA",
                    "FirstName": "John",
                    "LastName": "Heart"
                },
                "items": [
                    {
                        "itemType": "group",
                        "caption": "Personal Data",
                        "items": [ "FirstName", "LastName" ]
                    },
                    {
                        "itemType": "group",
                        "colCount": 2,
                        "caption": "Home Address",
                        "items": [ "Address", "City", "State" ]
                    }
                ]
            }
        }
    ]
};};
ThemeBuilder.__gallery_group = function() { return {
"id": "gallery",
"ord": 9,
"widgets": [
	{  
		"name": "dxGallery",
		"supportedThemes": ["all"],
		"options": { 
			"items": [
				"Content/Images/person1.png",
				"Content/Images/person2.png",
				"Content/Images/person3.png",
				"Content/Images/person4.png"
			],
			"showNavButtons": true,
            "height": 320,
            "width":  500
		}
	}
]
};};
ThemeBuilder.__layouts_desktop_group = function() { return {
    "id": "layouts.desktop",
    "application": {
        "layoutSet": [
            {
                "controllerExpr": "DevExpress.framework.html.DefaultLayoutController",
                "controllerOptions": {
                    "name": "desktop",
                    "disableViewLoadingState": true
                }
            }
        ],
        "layouts": [
            "Desktop"
        ],
        "views": [
            "list",
            "about",
            "details"
        ],
        "navigation": [
            {
                "title": "Items",
                "onExecute": "#list",
                "icon": "home"
            },
            {
                "title": "About",
                "onExecute": "#about",
                "icon": "info"
            }
        ],
        "startupView": "list",
        "supportedThemes": [
            "generic-dark",
            "generic-light",
            "generic-light-compact",
            "generic-dark-compact"
        ]
    }
};};
ThemeBuilder.__layouts_split_group = function() { return {
    "id": "layouts.split",
    "application": {
        "device": {
            "deviceType": "tablet"
        },
        "layoutSet": [
            {
                "platform": "ios",
                "controllerExpr": "DevExpress.framework.html.IOSSplitLayoutController"
            },
            {
                "platform": "generic",
                "controllerExpr": "DevExpress.framework.html.IOSSplitLayoutController"
            },
            {
                "platform": "android",
                "controllerExpr": "DevExpress.framework.html.AndroidSplitLayoutController"
            }
        ],
        "views": [
            "list",
            "details"
        ],
        "layouts": [
            "simple",
            "empty",
            "split"
        ],
        "startupView": ["list", "details/0"],
        "supportedThemes": [
            "ios7-default",
            "generic-dark",
            "generic-light",
            "android5-light",
            "generic-light-compact",
            "generic-dark-compact"
        ]
    }
};};
ThemeBuilder.__list_group = function() { return {
"id": "list",
	"ord": 2,
	"widgets": [
		{ 
			"name": "dxList",
			"supportedThemes": ["all"],
			"options": {
				"grouped": true,
				"editEnabled": true,
                "editConfig": {
                    "deleteEnabled": true,
                    "selectionEnabled": true
                },
                "selectionMode": "single",
				"items": [
					{
						"key": "Group A",
						"items": [
							"Russia",
							"Czech",
							"Poland",
							"Greece"
						]
					},
					{
						"key": "Group B",
						"items": [
							"Germany",
							"Portugal",
							"Denmark",
							"Netherlands"
						]
					}
				]
			}
		}
	]
};};
ThemeBuilder.__navigations_accordion_group = function() { return {
    "id": "navigations.accordion",
    "widgets": [
        {
			"name": "dxAccordion",
			"supportedThemes": ["all"],
			"options": {
				
				"items": [
                    {
                        "title": "Jack London (1876-1916)", 
                        "icon":  "card",
                        "html": "<p>The Call of the Wild (1903)</p><p>Before Adam (1907)</p><p>Burning Daylight (1910)</p><p>The Abysmal Brute (1911)</p>" },
                    {
                        "title": "Mark Twain (1835-1910)", 
                        "icon":  "card",
                        "html": "<p>Screamers (1871)</p><p>Eye Openers (1871)</p><p>Colonel Sellers (1874)</p><p>Merry Tales (1892)</p>" }
				]
			}
		}
    ]
};};
ThemeBuilder.__navigations_menu_group = function() { return {
    "id": "navigations.menu",
    "widgets": [
        {
            "name": "dxMenu",
            "supportedThemes": [ "all" ],
            "options": {
                "showFirstSubmenuMode": "onHover",
                "animation": {
                    "show": false,
                    "hide": false
                },
                "items": [
                    {
                        "text": ".NET",
                        "items": [
                            {
                                "text": "Individual Platforms",
                                "items": [
                                    { "text": "WinForms" },
                                    { "text": "ASP.NET" },
                                    { "text": "MVC" },
                                    { "text": "WPF" },
                                    { "text": "Silverlight" },
                                    { "text": "Windows 8 XAML" }
                                ],
                                "selected": true,
                                "selectable": true
                            },
                            {
                                "text": "Frameworks",
                                "items": [ { "text": "eXpressApp Framework" } ]
                            },
                            {
                                "text": "Code-Debug-Refactor",
                                "items": [ { "text": "CodeRush for Visual Studio" } ]
                            },
                            {
                                "text": "Mobile (HTML JS)",
                                "items": [ { "text": "DevExtreme" } ]
                            },
                            {
                                "text": "Cross-Platform",
                                "items": [
                                    { "text": "Reporting" },
                                    { "text": "Document Generation" }
                                ]
                            },
                            {
                                "text": "Enterprise Tools",
                                "items": [ { "text": "Report Server" },
                                    { "text": "Analytics Dashboard" } ]
                            }
                        ]
                    },
                    {
                        "text": "HTML JavaScript",
                        "items": [
                            {
                                "text": "Mobile",
                                "items": [ { "text": "Phone JS" } ]
                            },
                            {
                                "text": "HTML 5 JS Widgets",
                                "items": [ { "text": "Chart JS" } ]
                            }
                        ]
                    },
                    {
                        "text": "iOS 7",
                        "items": [
                            {
                                "text": "Native",
                                "items": [ { "text": "DataExplorer" } ]
                            }
                        ]
                    },
                    {
                        "text": "Testing Tools",
                        "items": [
                            {
                                "text": "Web Testing",
                                "items": [ { "text": "TestCafe" } ]
                            }
                        ]
                    },
                    {
                        "text": "Delphi & C++Builder",
                        "disabled": true
                    }
                ]
            }
        }
    ]
};};
ThemeBuilder.__navigations_navbar_group = function() { return {
"id": "navigations.navbar",
"widgets": [
    { 
	    "name": "dxNavBar",
	    "supportedThemes": ["all"],
	    "options": { 
		    "dataSource": [
			    { "text": "user", "icon": "user" },
			    { "text": "find", "icon": "find" },
			    { "text": "favorites", "icon": "favorites" },
			    { "text": "about", "icon": "info" },
			    { "text": "home", "icon": "home", "badge": 77 },
			    { "text": "URI", "icon": "tips", "disabled": true }
		    ]
	    }
    }
]
};};
ThemeBuilder.__navigations_tabs_group = function() { return {
"id": "navigations.tabs", 
"widgets": [
{ 
		"name": "dxTabs",
		"supportedThemes": ["all"],
		"options": {  
			"dataSource": [
				{ "text": "user", "icon": "user" },
				{ "text": "comment", "icon": "comment" },
				{ "text": "find", "icon": "find" },
				{ "text": "disabled", "disabled": true }
			] 
		}
	}
]
};};
ThemeBuilder.__navigations_toolbar_group = function() { return {
"id": "navigations.toolbar", 
"widgets": [
	{ 
		"name": "dxToolbar",
		"supportedThemes": ["all"],
		"options": {
			"dataSource":[
				{ "location": "before", "widget": "button", "options": { "type": "back", "text": "Back" }},
				{ "location": "after", "widget": "button", "options": { "icon": "plus" } },
				{ "location": "after", "widget": "button", "options": { "icon": "find" } },
				{ "location": "center", "text": "Products" }
			]
		}
	}
]
};};
ThemeBuilder.__navigations_treeview_group = function() { return {
    "id": "navigations.treeview",
    "widgets": [
        {
            "name": "dxTreeView",
            "supportedThemes": [ "all" ],
            "prerenderAction":  "MAKE_TREEVIEW_BORDER",
            "options": {
                "showCheckBoxes": true,
                "selectAllEnabled": true,
                "items": [
                    {
                        "id": 1,
                        "text": ".NET",
                        "items": [
                            {
                                "id": 11,
                                "text": "Individual Platforms",
                                "items": [
                                    { "id": 111, "text": "WinForms" },
                                    { "id": 112, "text": "ASP.NET" },
                                    { "id": 113, "text": "MVC" },
                                    { "id": 114, "text": "WPF" },
                                    { "id": 115, "text": "Silverlight" },
                                    { "id": 116, "text": "Windows 8 XAML" }
                                ],
                                "selected": true,
                                "expanded": true
                            },
                            {
                                "id": 12,
                                "text": "Frameworks",
                                "items": [
                                    { "id": 121, "text": "eXpressApp Framework" }
                                ]
                            },
                            {
                                "id": 13,
                                "text": "Code-Debug-Refactor",
                                "items": [
                                    { "id": 131, "text": "CodeRush for Visual Studio" }
                                ]
                            },
                            {
                                "id": 14,
                                "text": "Mobile (HTML JS)",
                                "items": [
                                    { "id": 141, "text": "DevExtreme" }
                                ]
                            },
                            {
                                "id": 15,
                                "text": "Cross-Platform",
                                "items": [
                                    { "id": 151, "text": "Reporting" },
                                    { "id": 152, "text": "Document Generation" }
                                ]
                            },
                            {
                                "id": 16,
                                "text": "Enterprise Tools",
                                "items": [
                                    { "id": 161, "text": "Report Server" },
                                    { "id": 162, "text": "Analytics Dashboard" }
                                ]
                            }
                        ]
                    },
                    {
                        "id": 2,
                        "text": "HTML JavaScript",
                        "items": [
                            {
                                "id": 21,
                                "text": "Mobile",
                                "items": [
                                    { "id": 211, "text": "Phone JS" }
                                ]
                            },
                            {
                                "id": 22,
                                "text": "HTML 5 JS Widgets",
                                "items": [
                                    { "id": 221, "text": "Chart JS" }
                                ]
                            }
                        ]
                    },
                    {
                        "id": 3,
                        "text": "iOS 7",
                        "items": [
                            {
                                "id": 31,
                                "text": "Native",
                                "items": [
                                    { "id": 311, "text": "DataExplorer" }
                                ]
                            }
                        ]
                    },
                    {
                        "id": 4,
                        "text": "Testing Tools",
                        "items": [
                            {
                                "id": 41,
                                "text": "Web Testing",
                                "items": [
                                    { "id": 441, "text": "TestCafe" }
                                ]
                            }
                        ]
                    },
                    {
                        "id": 5,
                        "text": "Delphi & C++Builder"
                    }
                ]
            }
        }
    ]
};};
ThemeBuilder.__overlays_actionsheet_group = function() { return {
"id": "overlays.actionsheet", 
"widgets": [
	{
		"name": "dxButton",
		"supportedThemes": ["ios7-default", "android5-light"],
		"options": {
			"text": "Show actionsheet",
			"click": "SHOW_ACTIONSHEET"
		}
	},
{
		"name": "dxActionSheet",
		"supportedThemes": ["ios7-default", "android5-light"],
		"options": {
			"items": [
				{ "text": "Reply" },
				{ "text": "ReplyAll" },
				{ "text": "Forward" },
				{ "text": "Delete" }
			],
			"visible": false,
			"title": "ActionSheet"
		}
	}
]
};};
ThemeBuilder.__overlays_common_group = function() { return {
"id": "overlays.common", 
"widgets": [
	{
		"name": "dxButton",
		"title": "dxLoadPanel",
		"supportedThemes": ["all"],
		"options": {
			"text": "Show load panel",
			"click": "SHOW_LOAD_PANEL"
		}
	},
	{
		"name": "dxButton",
		"title": "dxPopup",
		"id": "popup-button",
		"supportedThemes": ["all"],
		"options": {
			"text": "Show popup",
			"click": "SHOW_POPUP"
		}
	},

	{
		"title": "Confirm dialog",
		"name": "dxButton",
		"supportedThemes": ["ios7-default", "android5-light"],
		"options": {
			"text": "Show confirm dialog",
			"click": "SHOW_CONFIRM_DIALOG"
		}
	},
	{
		"name": "dxButton",
		"title": "dxActionSheet",
		"supportedThemes": ["generic-dark", "generic-light", "generic-carmine", "generic-darkmoon", "generic-softblue", "generic-darkviolet", "generic-greenmist", "generic-carmine-compact", "generic-darkmoon-compact", "generic-softblue-compact", "generic-darkviolet-compact", "generic-greenmist-compact", "generic-light-compact", "generic-dark-compact"],
		"options": {
			"text": "Show actionsheet",
			"click": "SHOW_ACTIONSHEET"
		}
	},
	{
		"name": "dxLoadPanel",
		"id": "load-panel-sample",
		"supportedThemes": ["all"],
		"options": { 
			"message": "Loading...",
			"closeOnOutsideClick": true,
			"visible": false
		}
	},
	{
		"name": "dxPopup",
		"id": "popup-sample",
		"supportedThemes": ["all"],
		"options": { 
			"closeOnOutsideClick": true,
			"visible": false,
			"showTitle": true,
			"title": "Popup title",
			"deferRendering": false
		}
	},
	{
		"name": "dxActionSheet",
		"supportedThemes": ["generic-dark", "generic-light", "generic-carmine", "generic-darkmoon", "generic-softblue", "generic-darkviolet", "generic-greenmist", "generic-carmine-compact", "generic-darkmoon-compact", "generic-softblue-compact", "generic-darkviolet-compact", "generic-greenmist-compact", "generic-light-compact", "generic-dark-compact"],
		"options": {
			"items": [
				{ "text": "Reply" },
				{ "text": "ReplyAll" },
				{ "text": "Forward" },
				{ "text": "Delete" }
			],
			"visible": false,
			"title": "ActionSheet"
		}
	}
]
};};
ThemeBuilder.__overlays_toasts_group = function() { return {
"id": "overlays.toasts", 
"widgets": [
	{
		"name": "dxButton",
		"title": "Info Type",
		"supportedThemes": ["all"],
		"options": {
			"text": "Show info toast",
			"click": "SHOW_INFO_TOAST"
		}
 	},
	{
		"name": "dxButton",
		"title": "Warning Type",
		"supportedThemes": ["all"],
		"options": {
			"text": "Show warning toast",
			"click": "SHOW_WARNING_TOAST"
		}
 	},
	{
		"name": "dxButton",
		"title": "Error Type",
		"supportedThemes": ["all"],
		"options": {
			"text": "Show error toast",
			"click": "SHOW_ERROR_TOAST"
		}
 	},
	{
		"name": "dxButton",
		"title": "Success Type",
		"supportedThemes": ["all"],
		"options": {
			"text": "Show success toast",
			"click": "SHOW_SUCCESS_TOAST"
		}
 	},
	{
		"name": "dxToast",
		"id": "success-toast-sample",
		"supportedThemes": ["all"],
		"options": { 
			"message": "The toast message",
			"type": "success",
			"closeOnOutsideClick": true,
			"displayTime": 3000
		}
	},
	{
		"name": "dxToast",
		"id": "error-toast-sample",
		"supportedThemes": ["all"],
		"options": { 
			"message": "The toast message",
			"type": "error",
			"closeOnOutsideClick": true,
			"displayTime": 3000
		}
	},
	{
		"name": "dxToast",
		"id": "warning-toast-sample",
		"supportedThemes": ["all"],
		"options": { 
			"message": "The toast message",
			"type": "warning",
			"closeOnOutsideClick": true,
			"displayTime": 3000
		}
	},
	{
		"name": "dxToast",
		"id": "info-toast-sample",
		"supportedThemes": ["all"],
		"options": { 
			"message": "The toast message",
			"type": "info",
			"closeOnOutsideClick": true,
			"displayTime": 3000
		}
	}
]
};};
ThemeBuilder.__overlays_tooltip_group = function() { return {
"id": "overlays.tooltip", 
"widgets": [
{
		"name": "dxButton",
		"id": "tooltip-button",
		"supportedThemes": ["all"],
		"options": {
			"text": "Show tooltip",
			"click": "SHOW_TOOLTIP"
		}
	},
	{
		"name": "dxTooltip",
		"id": "tooltip-sample",
		"supportedThemes": ["all"],
		"options": { 
			"target": "#tooltip-button",
			"closeOnOutsideClick": true,
			"visible": false,
			"showTitle": false,
			"deferRendering": false,
			"text": "Show tooltip"
		}
	}
]
};};
ThemeBuilder.__pivotgrid_group = function() { return {
"id": "pivotgrid", 
"widgets": [
	{
        "name": "dxPivotGrid",
        "supportedThemes": [ "generic-dark", "generic-light", "generic-carmine", "generic-darkmoon", "generic-softblue", "generic-darkviolet", "generic-greenmist", "generic-carmine-compact", "generic-darkmoon-compact", "generic-softblue-compact", "generic-darkviolet-compact", "generic-greenmist-compact", "generic-light-compact", "generic-dark-compact" ],
        "options": {
			"allowSorting": true,
            "allowSortingBySummary": true,
            "allowExpandAll": true,
            "allowFiltering": true,
            "height":  700,
			"dataSource": {
                    "fields": [
                        { "dataField": "OrderDate", "dataType": "date", "groupName": "OrderDate", "area": "column" },
                        { "dataField": "OrderDate", "expanded": true, "groupInterval": "year", "dataType": "date", "groupName": "OrderDate", "groupIndex": 0 },
                        { "dataField": "OrderDate", "groupInterval": "quarter", "dataType": "date", "groupName": "OrderDate", "groupIndex": 0 },
                        { "dataField": "OrderDate", "groupInterval": "month", "dataType": "date", "groupName": "OrderDate", "groupIndex": 0 },
                        { "dataField": "ShipCountry", "dataType": "string", "area": "row" },
                        { "dataField": "ShipCity", "dataType": "string", "area": "row" },
                        { "dataField": "ShipName", "dataType": "string", "area": "row" },
                        { "summaryType": "count", "caption": "Count", "area": "data" },
                        { "dataField": "Freight", "dataType": "number", "summaryType": "avg", "caption": "Avg Freight", "format": {"type":"fixedPoint", "precision": 2}, "area": "data" } 
                    ],
                    "store":[{"OrderID":10248,"CustomerID":"VINET","EmployeeID":5,"OrderDate":"1996/07/04","RequiredDate":"1996/08/01","ShippedDate":"1996/07/16","ShipVia":3,"Freight":"32.3800","ShipName":"Vins et alcools Chevalier","ShipAddress":"59 rue de l'Abbaye","ShipCity":"Reims","ShipRegion":null,"ShipPostalCode":"51100","ShipCountry":"France"},{"OrderID":10249,"CustomerID":"TOMSP","EmployeeID":6,"OrderDate":"1996/07/05","RequiredDate":"1996/08/16","ShippedDate":"1996/07/10","ShipVia":1,"Freight":"11.6100","ShipName":"Toms Spezialitaten","ShipAddress":"Luisenstr. 48","ShipCity":"Munster","ShipRegion":null,"ShipPostalCode":"44087","ShipCountry":"Germany"},{"OrderID":10250,"CustomerID":"HANAR","EmployeeID":4,"OrderDate":"1996/07/08","RequiredDate":"1996/08/05","ShippedDate":"1996/07/12","ShipVia":2,"Freight":"65.8300","ShipName":"Hanari Carnes","ShipAddress":"Rua do Paco, 67","ShipCity":"Rio de Janeiro","ShipRegion":"RJ","ShipPostalCode":"05454-876","ShipCountry":"Brazil"},{"OrderID":10251,"CustomerID":"VICTE","EmployeeID":3,"OrderDate":"1996/07/08","RequiredDate":"1996/08/05","ShippedDate":"1996/07/15","ShipVia":1,"Freight":"41.3400","ShipName":"Victuailles en stock","ShipAddress":"2, rue du Commerce","ShipCity":"Lyon","ShipRegion":null,"ShipPostalCode":"69004","ShipCountry":"France"},{"OrderID":10252,"CustomerID":"SUPRD","EmployeeID":4,"OrderDate":"1996/07/09","RequiredDate":"1996/08/06","ShippedDate":"1996/07/11","ShipVia":2,"Freight":"51.3000","ShipName":"Supremes delices","ShipAddress":"Boulevard Tirou, 255","ShipCity":"Charleroi","ShipRegion":null,"ShipPostalCode":"B-6000","ShipCountry":"Belgium"},{"OrderID":10253,"CustomerID":"HANAR","EmployeeID":3,"OrderDate":"1996/07/10","RequiredDate":"1996/07/24","ShippedDate":"1996/07/16","ShipVia":2,"Freight":"58.1700","ShipName":"Hanari Carnes","ShipAddress":"Rua do Paco, 67","ShipCity":"Rio de Janeiro","ShipRegion":"RJ","ShipPostalCode":"05454-876","ShipCountry":"Brazil"},{"OrderID":10254,"CustomerID":"CHOPS","EmployeeID":5,"OrderDate":"1996/07/11","RequiredDate":"1996/08/08","ShippedDate":"1996/07/23","ShipVia":2,"Freight":"22.9800","ShipName":"Chop-suey Chinese","ShipAddress":"Hauptstr. 31","ShipCity":"Bern","ShipRegion":null,"ShipPostalCode":"3012","ShipCountry":"Switzerland"},{"OrderID":10255,"CustomerID":"RICSU","EmployeeID":9,"OrderDate":"1996/07/12","RequiredDate":"1996/08/09","ShippedDate":"1996/07/15","ShipVia":3,"Freight":"148.3300","ShipName":"Richter Supermarkt","ShipAddress":"Starenweg 5","ShipCity":"Geneve","ShipRegion":null,"ShipPostalCode":"1204","ShipCountry":"Switzerland"},{"OrderID":10256,"CustomerID":"WELLI","EmployeeID":3,"OrderDate":"1996/07/15","RequiredDate":"1996/08/12","ShippedDate":"1996/07/17","ShipVia":2,"Freight":"13.9700","ShipName":"Wellington Importadora","ShipAddress":"Rua do Mercado, 12","ShipCity":"Resende","ShipRegion":"SP","ShipPostalCode":"08737-363","ShipCountry":"Brazil"},{"OrderID":10257,"CustomerID":"HILAA","EmployeeID":4,"OrderDate":"1996/07/16","RequiredDate":"1996/08/13","ShippedDate":"1996/07/22","ShipVia":3,"Freight":"81.9100","ShipName":"HILARION-Abastos","ShipAddress":"Carrera 22 con Ave. Carlos Soublette #8-35","ShipCity":"San Cristobal","ShipRegion":"Tachira","ShipPostalCode":"5022","ShipCountry":"Venezuela"},{"OrderID":10258,"CustomerID":"ERNSH","EmployeeID":1,"OrderDate":"1996/07/17","RequiredDate":"1996/08/14","ShippedDate":"1996/07/23","ShipVia":1,"Freight":"140.5100","ShipName":"Ernst Handel","ShipAddress":"Kirchgasse 6","ShipCity":"Graz","ShipRegion":null,"ShipPostalCode":"8010","ShipCountry":"Austria"},{"OrderID":10259,"CustomerID":"CENTC","EmployeeID":4,"OrderDate":"1996/07/18","RequiredDate":"1996/08/15","ShippedDate":"1996/07/25","ShipVia":3,"Freight":"3.2500","ShipName":"Centro comercial Moctezuma","ShipAddress":"Sierras de Granada 9993","ShipCity":"Mexico D.F.","ShipRegion":null,"ShipPostalCode":"05022","ShipCountry":"Mexico"},{"OrderID":10260,"CustomerID":"OTTIK","EmployeeID":4,"OrderDate":"1996/07/19","RequiredDate":"1996/08/16","ShippedDate":"1996/07/29","ShipVia":1,"Freight":"55.0900","ShipName":"Ottilies Kaseladen","ShipAddress":"Mehrheimerstr. 369","ShipCity":"Koln","ShipRegion":null,"ShipPostalCode":"50739","ShipCountry":"Germany"},{"OrderID":10261,"CustomerID":"QUEDE","EmployeeID":4,"OrderDate":"1996/07/19","RequiredDate":"1996/08/16","ShippedDate":"1996/07/30","ShipVia":2,"Freight":"3.0500","ShipName":"Que Delicia","ShipAddress":"Rua da Panificadora, 12","ShipCity":"Rio de Janeiro","ShipRegion":"RJ","ShipPostalCode":"02389-673","ShipCountry":"Brazil"},{"OrderID":10262,"CustomerID":"RATTC","EmployeeID":8,"OrderDate":"1996/07/22","RequiredDate":"1996/08/19","ShippedDate":"1996/07/25","ShipVia":3,"Freight":"48.2900","ShipName":"Rattlesnake Canyon Grocery","ShipAddress":"2817 Milton Dr.","ShipCity":"Albuquerque","ShipRegion":"NM","ShipPostalCode":"87110","ShipCountry":"USA"}]
                }
        }
    }
]
};};
ThemeBuilder.__progressbars_group = function() { return {
    "id": "progressbars",
    "widgets": [
        {
            "name": "dxProgressBar",
            "supportedThemes": [ "all" ],
            "options": {
                "value": 40,
                "min": 0,
                "max": 100,
                "showStatus": true
            }
        }
    ]
};};
ThemeBuilder.__scheduler_group = function() { return {
"id": "scheduler", 
"widgets": [
    {
        "name": "dxScheduler",
        "id": "scheduler-preview",
        "supportedThemes": [ "generic-dark", "generic-light", "generic-carmine", "generic-darkmoon", "generic-softblue", "generic-darkviolet", "generic-greenmist", "generic-carmine-compact", "generic-darkmoon-compact", "generic-softblue-compact", "generic-darkviolet-compact", "generic-greenmist-compact", "generic-light-compact", "generic-dark-compact" ],
        "options": {
            "views": [ "month", "week", "day" ],
            "currentDate": 1433106000000,
            "height": 700,
            "currentView": "week",
            "firstDayOfWeek": 1,
            "startDayHour": 2,
            "endDayHour": 11,
            "dataSource": [
                {
                    "startDate": 1433113200000,
                    "endDate":  1433115000000,
                    "text": "The first appointment"
                },
                {
                    "startDate": 1433296800000,
                    "endDate":  1433298600000,
                    "text": "The second appointment"
                }
            ]
        }
    },
    {
        "name": "dxScheduler",
        "id": "scheduler-preview",
        "supportedThemes":  [ "android5-light", "ios7-default" ],
        "options": {
            "views": [ "month", "week", "day" ],
            "currentDate":  1433106000000,
            "currentView": "week",
            "firstDayOfWeek": 1,
            "startDayHour": 2,
            "endDayHour": 6,
            "width": 600,
            "height":  470,
            "dataSource":  [
				{
                    "startDate": 1433113200000,
                    "endDate":  1433115000000,
				    "text": "The first appointment"
				},
				{
                    "startDate": 1433296800000,
                    "endDate":  1433298600000,
				    "text": "The second appointment"
				}
            ]
        }
    }
]
};};
ThemeBuilder.__scrollview_group = function() { return {
"id": "scrollview", 
"widgets": [
		{
			"name": "dxScrollView",
			"supportedThemes": ["all"],
			"prerenderAction": "LOAD_SCROLLVIEW_CONTENT",
			"options": { 
				"bounceEnabled": true	
			}
		}
]
};};
ThemeBuilder.__sliders_group = function() { return {
"id": "sliders",
"ord": 2,
"widgets": [
	{
		"name": "dxSlider",
		"title": "dxSlider",
		"supportedThemes": ["all"],
		"options": {
			"min": 0, 
			"max": 100, 
			"value": 38
		}
	},
	{
		"name": "dxRangeSlider",
		"title": "dxRangeSlider",
		"supportedThemes": ["all"],
		"options": {
			"min": 0, 
			"max": 100, 
			"start": 38, 
			"end": 62
		}
	}
]
};};
ThemeBuilder.__tileview_group = function() { return {
	"id": "tileview",
	"widgets": [
		{
			"name": "dxTileView",
			"supportedThemes": ["all"],
			"options": {
				"listHeight": 400,
				"width": "100%",
				"items": [
					"Russia",
					"Czech",
					"Poland",
					"Greece",
					"Germany",
					"Portugal",
					"Denmark",
					"Netherlands",
					"Belgium",
                    "Italy",
                    "France",
                    "Spain",
                    "Ukraine",
                    "Ireland",
                    "Austria",
                    "Sweden",
                    "Norway",
                    "Finland"
				]
			}
		}
	]
};};
ThemeBuilder.__treelist_group = function() { return {
  "id": "treelist",
  "ord": 5,
  "widgets": [
    {
      "name": "dxTreeList",
      "supportedThemes": [ "all" ],
      "options": {
        "columnChooser": { "enabled": true },
        "rowAlternationEnabled": true,
        "keyExpr": "ID",
        "parentIdExpr": "Head_ID",
        "expandedRowKeys": [ 1, 2, 5 ],
        "columnAutoWidth": true,
        "dataSource": [
          {
            "ID": 1,
            "Head_ID": 0,
            "Full_Name": "John Heart",
            "Prefix": "Mr.",
            "Title": "CEO",
            "City": "Los Angeles",
            "Email": "jheart@dx-email.com",
            "Skype": "jheart_DX_skype",
            "Mobile_Phone": "(213) 555-9392",
            "Birth_Date": "1964-03-16",
            "Hire_Date": "1995-01-15"
          },
          {
            "ID": 2,
            "Head_ID": 1,
            "Full_Name": "Samantha Bright",
            "Prefix": "Dr.",
            "Title": "COO",
            "City": "Los Angeles",
            "Email": "samanthab@dx-email.com",
            "Skype": "samanthab_DX_skype",
            "Mobile_Phone": "(213) 555-2858",
            "Birth_Date": "1966-05-02",
            "Hire_Date": "2004-05-24"
          },
          {
            "ID": 3,
            "Head_ID": 1,
            "Full_Name": "Arthur Miller",
            "Prefix": "Mr.",
            "Title": "CTO",
            "City": "Los Angeles",
            "Email": "arthurm@dx-email.com",
            "Skype": "arthurm_DX_skype",
            "Mobile_Phone": "(310) 555-8583",
            "Birth_Date": "1972-07-11",
            "Hire_Date": "2007-12-18"
          },
          {
            "ID": 4,
            "Head_ID": 1,
            "Full_Name": "Robert Reagan",
            "Prefix": "Mr.",
            "Title": "CMO",
            "City": "Pasadena",
            "Email": "robertr@dx-email.com",
            "Skype": "robertr_DX_skype",
            "Mobile_Phone": "(818) 555-2387",
            "Birth_Date": "1974-09-07",
            "Hire_Date": "2002-11-08"
          },
          {
            "ID": 5,
            "Head_ID": 1,
            "Full_Name": "Greta Sims",
            "Prefix": "Ms.",
            "Title": "HR Manager",
            "City": "Alhambra",
            "Email": "gretas@dx-email.com",
            "Skype": "gretas_DX_skype",
            "Mobile_Phone": "(818) 555-6546",
            "Birth_Date": "1977-11-22",
            "Hire_Date": "1998-04-23"
          },
          {
            "ID": 6,
            "Head_ID": 3,
            "Full_Name": "Brett Wade",
            "Prefix": "Mr.",
            "Title": "IT Manager",
            "City": "San Marino",
            "Email": "brettw@dx-email.com",
            "Skype": "brettw_DX_skype",
            "Mobile_Phone": "(626) 555-0358",
            "Birth_Date": "1968-12-01",
            "Hire_Date": "2009-03-06"
          },
          {
            "ID": 7,
            "Head_ID": 5,
            "Full_Name": "Sandra Johnson",
            "Prefix": "Mrs.",
            "Title": "Controller",
            "City": "Long Beach",
            "Email": "sandraj@dx-email.com",
            "Skype": "sandraj_DX_skype",
            "Mobile_Phone": "(562) 555-2082",
            "Birth_Date": "1974-11-15",
            "Hire_Date": "2005-05-11"
          },
          {
            "ID": 8,
            "Head_ID": 4,
            "Full_Name": "Ed Holmes",
            "Prefix": "Dr.",
            "Title": "Sales Manager",
            "City": "Malibu",
            "Email": "edwardh@dx-email.com",
            "Skype": "edwardh_DX_skype",
            "Mobile_Phone": "(310) 555-1288",
            "Birth_Date": "1973-07-14",
            "Hire_Date": "2005-06-19"
          },
          {
            "ID": 9,
            "Head_ID": 3,
            "Full_Name": "Barb Banks",
            "Prefix": "Mrs.",
            "Title": "Support Manager",
            "City": "Pacific Palisades",
            "Email": "barbarab@dx-email.com",
            "Skype": "barbarab_DX_skype",
            "Mobile_Phone": "(310) 555-3355",
            "Birth_Date": "1979-04-14",
            "Hire_Date": "2002-08-07"
          },
          {
            "ID": 10,
            "Head_ID": 2,
            "Full_Name": "Kevin Carter",
            "Prefix": "Mr.",
            "Title": "Shipping Manager",
            "City": "Los Angeles",
            "Email": "kevinc@dx-email.com",
            "Skype": "kevinc_DX_skype",
            "Mobile_Phone": "(213) 555-2840",
            "Birth_Date": "1978-01-09",
            "Hire_Date": "2009-08-11"
          },
          {
            "ID": 11,
            "Head_ID": 5,
            "Full_Name": "Cindy Stanwick",
            "Prefix": "Ms.",
            "Title": "HR Assistant",
            "City": "Glendale",
            "Email": "cindys@dx-email.com",
            "Skype": "cindys_DX_skype",
            "Mobile_Phone": "(818) 555-6655",
            "Birth_Date": "1985-06-05",
            "Hire_Date": "2008-03-24"
          },
          {
            "ID": 12,
            "Head_ID": 8,
            "Full_Name": "Sammy Hill",
            "Prefix": "Mr.",
            "Title": "Sales Assistant",
            "City": "Pasadena",
            "Email": "sammyh@dx-email.com",
            "Skype": "sammyh_DX_skype",
            "Mobile_Phone": "(626) 555-7292",
            "Birth_Date": "1984-02-17",
            "Hire_Date": "2012-02-01"
          },
          {
            "ID": 13,
            "Head_ID": 10,
            "Full_Name": "Davey Jones",
            "Prefix": "Mr.",
            "Title": "Shipping Assistant",
            "City": "Pasadena",
            "Email": "davidj@dx-email.com",
            "Skype": "davidj_DX_skype",
            "Mobile_Phone": "(626) 555-0281",
            "Birth_Date": "1983-03-06",
            "Hire_Date": "2011-04-24"
          },
          {
            "ID": 14,
            "Head_ID": 10,
            "Full_Name": "Victor Norris",
            "Prefix": "Mr.",
            "Title": "Shipping Assistant",
            "City": "Los Angeles",
            "Email": "victorn@dx-email.com",
            "Skype": "victorn_DX_skype",
            "Mobile_Phone": "(213) 555-9278",
            "Birth_Date": "1986-07-23",
            "Hire_Date": "2012-07-23"
          },
          {
            "ID": 15,
            "Head_ID": 10,
            "Full_Name": "Mary Stern",
            "Prefix": "Ms.",
            "Title": "Shipping Assistant",
            "City": "Glendale",
            "Email": "marys@dx-email.com",
            "Skype": "marys_DX_skype",
            "Mobile_Phone": "(818) 555-7857",
            "Birth_Date": "1982-04-08",
            "Hire_Date": "2012-08-12"
          },
          {
            "ID": 16,
            "Head_ID": 10,
            "Full_Name": "Robin Cosworth",
            "Prefix": "Mrs.",
            "Title": "Shipping Assistant",
            "City": "Los Angeles",
            "Email": "robinc@dx-email.com",
            "Skype": "robinc_DX_skype",
            "Mobile_Phone": "(818) 555-0942",
            "Birth_Date": "1981-06-12",
            "Hire_Date": "2012-09-01"
          },
          {
            "ID": 17,
            "Head_ID": 9,
            "Full_Name": "Kelly Rodriguez",
            "Prefix": "Ms.",
            "Title": "Support Assistant",
            "City": "Glendale",
            "Email": "kellyr@dx-email.com",
            "Skype": "kellyr_DX_skype",
            "Mobile_Phone": "(818) 555-9248",
            "Birth_Date": "1988-05-11",
            "Hire_Date": "2012-10-13"
          },
          {
            "ID": 18,
            "Head_ID": 9,
            "Full_Name": "James Anderson",
            "Prefix": "Mr.",
            "Title": "Support Assistant",
            "City": "Los Angeles",
            "Email": "jamesa@dx-email.com",
            "Skype": "jamesa_DX_skype",
            "Mobile_Phone": "(323) 555-4702",
            "Birth_Date": "1987-01-29",
            "Hire_Date": "2012-10-18"
          },
          {
            "ID": 19,
            "Head_ID": 9,
            "Full_Name": "Antony Remmen",
            "Prefix": "Mr.",
            "Title": "Support Assistant",
            "City": "San Pedro",
            "Email": "anthonyr@dx-email.com",
            "Skype": "anthonyr_DX_skype",
            "Mobile_Phone": "(310) 555-6625",
            "Birth_Date": "1986-02-19",
            "Hire_Date": "2013-01-19"
          },
          {
            "ID": 20,
            "Head_ID": 8,
            "Full_Name": "Olivia Peyton",
            "Prefix": "Mrs.",
            "Title": "Sales Assistant",
            "City": "San Pedro",
            "Email": "oliviap@dx-email.com",
            "Skype": "oliviap_DX_skype",
            "Mobile_Phone": "(310) 555-2728",
            "Birth_Date": "1981-06-03",
            "Hire_Date": "2012-05-14"
          },
          {
            "ID": 21,
            "Head_ID": 6,
            "Full_Name": "Taylor Riley",
            "Prefix": "Mr.",
            "Title": "Network Admin",
            "City": "West Hollywood",
            "Email": "taylorr@dx-email.com",
            "Skype": "taylorr_DX_skype",
            "Mobile_Phone": "(310) 555-7276",
            "Birth_Date": "1982-08-14",
            "Hire_Date": "2012-04-14"
          },
          {
            "ID": 22,
            "Head_ID": 6,
            "Full_Name": "Amelia Harper",
            "Prefix": "Mrs.",
            "Title": "Network Admin",
            "City": "Los Angeles",
            "Email": "ameliah@dx-email.com",
            "Skype": "ameliah_DX_skype",
            "Mobile_Phone": "(213) 555-4276",
            "Birth_Date": "1983-11-19",
            "Hire_Date": "2011-02-10"
          },
          {
            "ID": 23,
            "Head_ID": 6,
            "Full_Name": "Wally Hobbs",
            "Prefix": "Mr.",
            "Title": "Programmer",
            "City": "Chatsworth",
            "Email": "wallyh@dx-email.com",
            "Skype": "wallyh_DX_skype",
            "Mobile_Phone": "(818) 555-8872",
            "Birth_Date": "1984-12-24",
            "Hire_Date": "2011-02-17"
          },
          {
            "ID": 24,
            "Head_ID": 6,
            "Full_Name": "Brad Jameson",
            "Prefix": "Mr.",
            "Title": "Programmer",
            "City": "San Fernando",
            "Email": "bradleyj@dx-email.com",
            "Skype": "bradleyj_DX_skype",
            "Mobile_Phone": "(818) 555-4646",
            "Birth_Date": "1988-10-12",
            "Hire_Date": "2011-03-02"
          },
          {
            "ID": 25,
            "Head_ID": 6,
            "Full_Name": "Karen Goodson",
            "Prefix": "Miss",
            "Title": "Programmer",
            "City": "South Pasadena",
            "Email": "kareng@dx-email.com",
            "Skype": "kareng_DX_skype",
            "Mobile_Phone": "(626) 555-0908",
            "Birth_Date": "1987-04-26",
            "Hire_Date": "2011-03-14"
          },
          {
            "ID": 26,
            "Head_ID": 5,
            "Full_Name": "Marcus Orbison",
            "Prefix": "Mr.",
            "Title": "Travel Coordinator",
            "City": "Los Angeles",
            "Email": "marcuso@dx-email.com",
            "Skype": "marcuso_DX_skype",
            "Mobile_Phone": "(213) 555-7098",
            "Birth_Date": "1982-03-02",
            "Hire_Date": "2005-05-19"
          },
          {
            "ID": 27,
            "Head_ID": 5,
            "Full_Name": "Sandy Bright",
            "Prefix": "Ms.",
            "Title": "Benefits Coordinator",
            "City": "Tujunga",
            "Email": "sandrab@dx-email.com",
            "Skype": "sandrab_DX_skype",
            "Mobile_Phone": "(818) 555-0524",
            "Birth_Date": "1983-09-11",
            "Hire_Date": "2005-06-04"
          },
          {
            "ID": 28,
            "Head_ID": 6,
            "Full_Name": "Morgan Kennedy",
            "Prefix": "Mrs.",
            "Title": "Graphic Designer",
            "City": "San Fernando Valley",
            "Email": "morgank@dx-email.com",
            "Skype": "morgank_DX_skype",
            "Mobile_Phone": "(818) 555-8238",
            "Birth_Date": "1984-07-17",
            "Hire_Date": "2012-01-11"
          },
          {
            "ID": 29,
            "Head_ID": 28,
            "Full_Name": "Violet Bailey",
            "Prefix": "Ms.",
            "Title": "Jr Graphic Designer",
            "City": "La Canada",
            "Email": "violetb@dx-email.com",
            "Skype": "violetb_DX_skype",
            "Mobile_Phone": "(818) 555-2478",
            "Birth_Date": "1985-06-10",
            "Hire_Date": "2012-01-19"
          },
          {
            "ID": 30,
            "Head_ID": 5,
            "Full_Name": "Ken Samuelson",
            "Prefix": "Dr.",
            "Title": "Ombudsman",
            "City": "Santa Fe Springs",
            "Email": "kents@dx-email.com",
            "Skype": "kents_DX_skype",
            "Mobile_Phone": "(562) 555-9282",
            "Birth_Date": "1972-09-11",
            "Hire_Date": "2009-04-22"
          }
        ],
        "filterRow": {
          "visible": true
        },
        "height": "500px",
        "searchPanel": {
          "visible": true
        },
        "selection": {
          "mode": "multiple"
        },
        "hoverStateEnabled": true,
        "allowColumnResizing": true,
        "allowColumnReordering": true,
        "editing": {
          "allowUpdating": true,
          "allowDeleting": true,
          "mode": "batch"
        },
        "columns": [
          {
            "dataField": "Title",
            "caption": "Position"
          },
          "Full_Name",
          "City",
          {
            "dataField": "Hire_Date",
            "dataType": "date"
          }
        ]
      }
    }
  ]
};};
