ORE Dashboard v.0.1 Alpha 19/Jun/2017
=====================================
This repository contains prototypes of front ends for ORE, the Open Source Risk Engine by Quaternion. (http://www.opensourcerisk.org/).

It currently contains a Visual Studio 2017 solution JMCASPNET2.

You need to download and install ORE before the solution will work. 
Once you have successfully built the solution, you just need to edit the
following AppSettings in the Web.Config configuration file.

With the default installation of ORE, you should only need to change ORERoot to
point to the root folder of the ORE installation.

    <!-- Custom App Settings -->
    <add key="ShellCmdExe" value="C:\Windows\System32\cmd.exe"/>
    <add key="ORERoot" value="C:\work\ORE-1.8.2\"/>
    
     <!-- These are relative paths from ORERoot -->
    <add key="OREExe" value="App\bin\x64\Release\ore.exe"/>
    <add key="InputPath" value="Examples\Example_1"/>
    <add key="TradeInputFile" value="Examples\Example_1\Input\portfolio_swap.xml"/>    
    <add key="OREConfigurationFile" value="Examples\Example_1\Input\ore.xml"/>   
    <add key="NPVResultsFile" value="Examples\Example_1\Output\npv.csv"/>    
    <add key="ExposureResultsFile" value="Examples\Example_1\Output\exposure_trade_Swap_20y.csv"/>     

Currently the functionality is restricted to entering an Interest Rate Swap, constrained to the market data is Example_1 of the ORE examples.

Click on the IRSwap tab to edit the swap and run the calculation.
Click on the NPVResults tab to view the exposure profile.


For queries contact:
jonathanmcc@eircom.net
2017 Abbot Computing Limited.
