// depends on Chart.js
var chart = angular.module('chartApp', ['chart.js']);

var app = angular.module('oreApp', []);
app.controller('oreCtrl', function ($scope, $http, $window) {

    $scope.LegTypes = [];
    $scope.DayCounts = [];
    $scope.PaymentConventions = [];
    $scope.ScheduleRules = [];
    $scope.Tenors = [];

    this.$onInit = function () {

        // static data values
        // these should ideally be read in from the server
        $scope.BooleanValues = [
            { id: "true", text: "true" },
            { id: "false", text: "false" }
        ];

        $scope.LegTypes = [
            { id: "Fixed", text: "Fixed" },
            { id: "Floating", text: "Floating" }
        ];

        $scope.DayCounts = [
            { id: "A360", text: "Act/360" },
            { id: "30/360", text: "30/360" },
            { id: "30E/360", text: "30E/360" },
            { id: "30/360 (Italian)", text: "30/360 (Italian)" },
            { id: "ActActISDA", text: "ActActISDA" },
            { id: "ActActISMA", text: "Act/Act (ISMA)" },
            { id: "Act/ActAFB", text: "Act/Act (AFB)" }
        ];

        $scope.PaymentConventions = [
            { id: "F", text: "Following" },
            { id: "MF", text: "Modified Following" },
            { id: "P", text: "Preceding" },
            { id: "MP", text: "Modified Preceding" },
            { id: "Unadjusted", text: "Unadjusted" }
        ];

        $scope.ScheduleRules = [
            { id: "Backward", text: "Backward" },
            { id: "Forward", text: "Forward" },
            { id: "Zero", text: "Zero" },
            { id: "ThirdWednesday", text: "Third Wednesday" },
            { id: "Twentieth", text: "Twentieth" },
            { id: "TwentiethIMM", text: "TwentiethIMM" },
            { id: "OldCDS", text: "Old CDS" },
            { id: "CDS", text: "CDS" }
        ];

        $scope.Tenors = [
            { id: "D", text: "Day" },
            { id: "W", text: "Week" },
            { id: "M", text: "Month" },
            { id: "Y", text: "Year" }
        ];

        // static market data
        // these should ideally be read in from the server
        $scope.Currencies = [
            { id: "CHF", text: "CHF" },
            { id: "EUR", text: "EUR" },
            { id: "GBP", text: "GBP" },
            { id: "JPY", text: "JPY" },
            { id: "SEK", text: "SEK" },
            { id: "USD", text: "USD" },
        ];

        // indexes
        $scope.Indexes = [
            { id: "EUR-EURIBOR-6M", text: "EUR-EURIBOR-6M" },
            { id: "GBP-LIBOR-6M", text: "GBP-LIBOR-6M" },
            { id: "USD-LIBOR-3M", text: "USD-LIBOR-3M" }
        ];

        // calendars
        $scope.Calendars = [
            { id: "AU", text: "AU" },
            { id: "CA", text: "CA" },
            { id: "CZK", text: "CZK" },
            { id: "DEN", text: "DEN" },
            { id: "HUF", text: "HUF" },
            { id: "JP", text: "JP" },
            { id: "NOK", text: "NOK" },
            { id: "NZD", text: "NZD" },
            { id: "PLN", text: "PLN" },
            { id: "SGD", text: "SGD" },
            { id: "SS", text: "SS" },
            { id: "TARGET", text: "TARGET" },
            { id: "UK", text: "UK" },
            { id: "US", text: "US" },
            { id: "WeekendsOnly", text: "WeekendsOnly" },
            { id: "ZAR", text: "ZAR" }
        ];

        var today = new Date();
        var months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
        $scope.dateString = today.getDate() + "/" + months[today.getMonth()] + "/" + today.getFullYear();

        $scope.Swap = new IRSwap();
        $scope.Results = new NPVResults();
        $scope.ExposureResults = [];
    };

    function Schedule() {
        this.StartDate = "20160301",
        this.EndDate = "20360301",
        this.Tenor = 6,
        this.TenorPeriod = { id: "M", text: "Month" },
        this.Calendar = { id: "TARGET", text: "TARGET" },
        this.Convention = { id: "F", text: "Following" },
        this.TermConvention = { id: "F", text: "Following" },
        this.Rule = { id: "Forward", text: "Forward" },
        this.EndOfMonth = "",
        this.FirstDate = "",
        this.LastDate = "";
    }

    function IRSwap() {

        this.Details = {
            TradeId: "Swap_20y",
            TradeType: "Swap",
            CounterParty: "CPTY_A",
            NettingSetId: "CPTY_A",

            PayLeg: {

                LegType: { id: "Floating", text: "Floating" },
                Currency: { id: "EUR", text: "EUR" },
                Notional: "10000000",
                DayCounter: { id: "A360", text: "Act/360" },
                PaymentConvention: { id: "MF", text: "Modified Following" },

                Rate: 0.0,
                Index: { id: "EUR-EURIBOR-6M", text: "EUR-EURIBOR-6M" },
                Spread: 0.0,
                IsInArrears: { id: "false", text: "false" },
                FixingDays: "2",

                Schedule: new Schedule()
            },

            ReceiveLeg: {

                LegType: { id: "Fixed", text: "Fixed" },
                Currency: { id: "EUR", text: "EUR" },
                Notional: "10000000",
                DayCounter: { id: "30/360", text: "30/360" },
                PaymentConvention: { id: "F", text: "Following" },

                Rate: 0.02,
                Index: "",
                Spread: 0.0,
                IsInArrears: { id: "false", text: "false" },
                FixingDays: "0",

                Schedule: new Schedule()
            }
        };
    }

    $scope.isFixedLeg = function(isPay)
    {
        if (isPay)
            return $scope.Swap.Details.PayLeg.LegType.text == 'Fixed';
        else
            return $scope.Swap.Details.ReceiveLeg.LegType.text == 'Fixed';
    }

    function NPVResults() {
        this.TradeId = "";
        this.TradeType = "";
        this.Maturity = "";
        this.MaturityTime = 0;
        this.NPV = 0;
        this.NpvCurrency = "";
        this.NPVBase = 0;
        this.BaseCurrency = "";
        this.Notional = 0;
        this.NotionalBase = 0;
    }

    function ExposureResult() {
        this.TradeId = "";
        this.Date = "";
        this.Time = 0;
        this.EPE = 0;
        this.ENE = 0;
        this.AllocatedEPE = 0;
        this.AllocatedENE = 0;
        this.PFE = 0;
        this.BaselEE = 0;
        this.BaselEEE = 0;
    }

    function GetResults() {

        $http({
            method: "GET",
            url: "/api/Results/1",
            dataType: 'json',
            headers: { "Content-Type": "application/json" }

        }).then(function successCallback(response) {

            var results = JSON.parse(response.data);

            $scope.Results.TradeId = results[0].TradeId;
            $scope.Results.TradeType = results[0].TradeType;
            $scope.Results.Maturity = results[0].Maturity;
            $scope.Results.MaturityTime = results[0].MaturityTime;
            $scope.Results.NPV = results[0].NPV;
            $scope.Results.NpvCurrency = results[0].NpvCurrency;
            $scope.Results.NPVBase = results[0].NPVBase;
            $scope.Results.BaseCurrency = results[0].BaseCurrency;
            $scope.Results.Notional = results[0].Notional;
            $scope.Results.NotionalBase = results[0].NotionalBase;

        }, function errorCallback(response) {
            $window.alert("ORE: Results Error (" + response.data + ")");
        });
    }

    function GetExposureResults() {

        $http({
            method: "GET",
            url: "/api/ExposureResults/1",
            dataType: 'json',
            headers: { "Content-Type": "application/json" }

        }).then(function successCallback(response) {

            $scope.ExposureResults = [];
            var data = JSON.parse(response.data);

            for (var i = 0; i < data.Results.length; i++) {

                var result = new ExposureResult();

                result.TradeId = data.Results[i].TradeId;
                result.Date = data.Results[i].Date;
                result.Time = data.Results[i].Time;
                result.EPE = data.Results[i].EPE;
                result.ENE = data.Results[i].ENE;
                result.AllocatedEPE = data.Results[i].AllocatedEPE;
                result.AllocatedENE = data.Results[i].AllocatedENE;
                result.PFE = data.Results[i].PFE;
                result.BaselEE = data.Results[i].BaselEE;
                result.BaselEEE = data.Results[i].BaselEEE;

                $scope.ExposureResults.push(result);
            }

            UpdateChart();

        }, function errorCallback(response) {
            $window.alert("ORE: Results Error (" + response.data + ")");
        });
    }

    function UpdateNPVStatus(text)
    {
        $("#RunStatus").html(text);
    }

    function UpdateChart() {

        var times = [];
        var epes = [];
        var enes = [];
        var pfes = [];

        for (var i = 0; i < $scope.ExposureResults.length; i++) {
            times.push($scope.ExposureResults[i].Time);
            epes.push($scope.ExposureResults[i].EPE);
            enes.push($scope.ExposureResults[i].ENE);
            pfes.push($scope.ExposureResults[i].PFE);
        }

        var ctx = $("#exposureChart");

        var myChart = new Chart(ctx, {
            type: 'line',
            series: ['EPE', 'ENE', 'PFE'],
            data: {
                xLabels: times,
                yLabels: [0, 25000, 50000, 75000, 100000, 125000, 150000, 175000, 200000, 225000, 250000],
                datasets: [{
                    label: "EPE",
                    borderWidth: 1,
                    borderColor: "#F00",
                    data: epes
                },
                {
                    label: "ENE",                    
                    borderWidth: 1,
                    borderColor: "#0F0",
                    data: enes
                },
                {
                    label: "PFE",
                    borderWidth: 1,
                    borderColor: "#00F",
                    data: pfes
                }]
            }
        });
    }

    //Submit the swap details to the server for processing by ORE
    $scope.ButtonClick = function (data) {

        //$window.alert(JSON.stringify($scope.Swap));
        UpdateNPVStatus("Calculating NPV ...");

        $http({

            method: "POST",
            url: "/Home/RunORE",
            dataType: 'json',
            data: { tradeSpecification: JSON.stringify($scope.Swap) },
            headers: { "Content-Type": "application/json" }

        }).then(function successCallback(response) {
            var success = JSON.parse(response.data);
            if (success) {
                GetResults();
                UpdateNPVStatus("NPV calculated.");
            }
            else {
                UpdateNPVStatus("NPV calculation failed.");
            }

        }, function errorCallback(response) {
            UpdateNPVStatus("NPV calculation error.");
        });
    };

    // Get the NPV Results
    $scope.RefreshButtonClick = function (data) {
        GetResults();
        GetExposureResults();
    };
});
