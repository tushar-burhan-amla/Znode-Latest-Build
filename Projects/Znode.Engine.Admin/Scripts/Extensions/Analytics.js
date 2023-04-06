gapi.analytics.ready(function () {

    // Authorize the user with an access token obtained server side.
    gapi.analytics.auth.authorize({
        'serverAuth': {
            'access_token': $('#AnalyticsAccessToken').val()
        }
    });

    // Create a new ActiveUsers instance to be rendered inside of an element with the id "active-users-container" and poll for changes every five seconds.
    var activeUsers = new gapi.analytics.ext.ActiveUsers({
        container: 'active-users-container',
        pollingInterval: 5
    });

    // Add CSS animation to visually show the when users come and go.
    activeUsers.once('success', function () {
        var timeout;
        this.on('change', function (data) {
            var element = this.container.firstChild;
            var animationClass = data.delta > 0 ? 'is-increasing' : 'is-decreasing';
            element.className += (' ' + animationClass);

            clearTimeout(timeout);
            timeout = setTimeout(function () {
                element.className =
                    element.className.replace(/ is-(increasing|decreasing)/g, '');
            }, 3000);
        });
    });

    // Create a new ViewSelector2 instance to be rendered inside of an element with the id "view-selector-container".
    var viewSelector = new gapi.analytics.ext.ViewSelector2({
        container: 'view-selector-container',
    }).execute();

    // Update the activeUsers component, the Chartjs charts, and the dashboard title whenever the user changes the view.
    viewSelector.on('viewChange', function (data) {
        // Start tracking active users for this view.
        activeUsers.set(data).execute();

        // Render all the of charts for this view.
        renderNewUsersChart(data.ids);
        renderBounceRateChart(data.ids);
        renderKeywordsChart(data.ids);
        renderPageViewsChart(data.ids);
        renderTopBrowsersChart(data.ids);
    });

    // Function to render new users chart
    function renderNewUsersChart(viewId) {
        var newUsersChart = new gapi.analytics.googleCharts.DataChart({
            query: {
                'ids': viewId,
                'start-date': Constant.analyticsChartStartDate,
                'end-date': Constant.analyticsChartEndDate,
                'metrics': 'ga:users,ga:newUsers',
                'dimensions': 'ga:date'
            },
            chart: {
                'container': 'new-users-container',
                'type': 'LINE',
                'options': {
                    'width': '100%'
                }
            }
        });
        newUsersChart.execute();
    }

    // Function to render bounce rate chart
    function renderBounceRateChart(viewId) {
        var bounceRateChart = new gapi.analytics.googleCharts.DataChart({
            query: {
                'ids': viewId,
                'start-date': Constant.analyticsChartStartDate,
                'end-date': Constant.analyticsChartEndDate,
                'metrics': 'ga:bounceRate',
                'dimensions': 'ga:date'
            },
            chart: {
                'container': 'bounce-rate-container',
                'type': 'LINE',
                'options': {
                    'width': '100%'
                }
            }
        });
        bounceRateChart.execute();
    }

    // Function to render keywords chart
    function renderKeywordsChart(viewId) {
        var keywordsChart = new gapi.analytics.googleCharts.DataChart({
            query: {
                'ids': viewId,
                'start-date': Constant.analyticsChartStartDate,
                'end-date': Constant.analyticsChartEndDate,
                'metrics': 'ga:users,ga:newUsers',
                'dimensions': 'ga:keyword',
                'sort': '-ga:keyword',
                'max-results': Constant.analyticsTableChartMaxResults
            },
            chart: {
                'container': 'keywords-container',
                'type': 'TABLE',
                'options': {
                    'width': '100%'
                }
            }
        });
        keywordsChart.execute();
    }

    // Function to render page views chart
    function renderPageViewsChart(viewId) {
        var pageViewsChart = new gapi.analytics.googleCharts.DataChart({
            query: {
                'ids': viewId,
                'start-date': Constant.analyticsChartStartDate,
                'end-date': Constant.analyticsChartEndDate,
                'metrics': 'ga:pageviews,ga:pageValue',
                'dimensions': 'ga:pagePath',
                'sort': '-ga:pageviews',
                'max-results': Constant.analyticsTableChartMaxResults
            },
            chart: {
                'container': 'page-views-container',
                'type': 'TABLE',
                'options': {
                    'width': '100%'
                }
            }
        });
        pageViewsChart.execute();
    }

    // Function to render top browsers chart
    function renderTopBrowsersChart(viewId) {
        var topBrowsersChart = new gapi.analytics.googleCharts.DataChart({
            query: {
                'ids': viewId,
                'start-date': Constant.analyticsChartStartDate,
                'end-date': Constant.analyticsChartEndDate,
                'metrics': 'ga:users',
                'dimensions': 'ga:browser',
                'sort': '-ga:users',
                'max-results': Constant.analyticsTableChartMaxResults
            },
            chart: {
                'container': 'top-browsers-container',
                'type': 'TABLE',
                'options': {
                    'width': '100%'
                }
            }
        });
        topBrowsersChart.execute();
    }
});
