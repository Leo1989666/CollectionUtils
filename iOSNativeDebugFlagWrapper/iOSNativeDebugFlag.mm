#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

extern "C"
{
    // For General Debug.Log
    bool GetEnableDebugLogFlag()
    {
        return YES;
    }
    ////
    
    // For Enabled SRDebugger
    bool GetEnableSRDebuggerFlag()
    {
        return YES;
    }
    
    // For Analytics
    bool GetEnableAnalyticsFlag()
    {
        return YES;
    }    
    ////
}
