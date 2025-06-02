//
//  CheckMouseSelectedRangeText.swift
//  TabbyCatBridge
//
//  Created by 方留军 on 2025/5/31.
//

import Foundation
import Cocoa
import ApplicationServices
import AppKit

@_cdecl("get_message")
public func get_message()->UnsafePointer<CChar>{
    let s = "hello from swift";
    guard let cstr = strdup(s) else{
        return "".withCString{$0}
    }
    return UnsafePointer(cstr);
}

/*
 获得是否选中了文本
 */
@_cdecl("get_selected_text")
public func get_selected_text()-> UnsafePointer<CChar>? {
    
   
        if !AXIsProcessTrusted() {
            return nil
        }
        
        
        let systemWideElement: AXUIElement = AXUIElementCreateSystemWide()
        
        var focusedWindow: AnyObject?
        
        var error: AXError = AXUIElementCopyAttributeValue(systemWideElement,
                                                           kAXFocusedApplicationAttribute as CFString,
                                                           &focusedWindow)
    
        
        if error != .success {
            return nil
//            let txt = getSelectedTextWithoutAffectingClipboard()
//            if(txt == nil)
//            {
//                return nil
//            }
//            else{
//                return UnsafePointer(strdup(txt));
//            }
        }
        
        if let focusedApp = focusedWindow as! AXUIElement? {
            var focusedElement: AnyObject?
            error = AXUIElementCopyAttributeValue(focusedApp,
                                                  kAXFocusedUIElementAttribute as CFString,
                                                  &focusedElement)
            
            
            if error == .success, let focusedElement = focusedElement as! AXUIElement? {
                
                var selectedTextValue: AnyObject?
                error = AXUIElementCopyAttributeValue(focusedElement,
                                                      kAXSelectedTextAttribute as CFString,
                                                      &selectedTextValue)
                if error == .success, let selectedText = selectedTextValue as? String {
                    if(selectedText==""){
                        return nil
                            
                    }
                    return UnsafePointer(strdup(selectedText));
                } else {
                    
                    return nil
                }
            }
        }
        return nil

}


func getSelectedTextWithoutAffectingClipboard() -> String? {
    let pasteboard = NSPasteboard.general

       // 1. 保存当前剪贴板文本
       let originalText = pasteboard.string(forType: .string)

       // 2. 模拟 ⌘C
       let src = CGEventSource(stateID: .combinedSessionState)
       let cmdDown = CGEvent(keyboardEventSource: src, virtualKey: 0x37, keyDown: true) // ⌘
       let cDown = CGEvent(keyboardEventSource: src, virtualKey: 0x08, keyDown: true)    // C
       let cUp = CGEvent(keyboardEventSource: src, virtualKey: 0x08, keyDown: false)
       let cmdUp = CGEvent(keyboardEventSource: src, virtualKey: 0x37, keyDown: false)

       cmdDown?.flags = .maskCommand
       cDown?.flags = .maskCommand

       cmdDown?.post(tap: .cghidEventTap)
       cDown?.post(tap: .cghidEventTap)
       cUp?.post(tap: .cghidEventTap)
       cmdUp?.post(tap: .cghidEventTap)

       // 3. 等待剪贴板更新
       usleep(100000)

       // 4. 获取选中的文本
       let copiedText = pasteboard.string(forType: .string)

       // 5. 恢复剪贴板原始文本
       pasteboard.clearContents()
       if let originalText = originalText {
           pasteboard.setString(originalText, forType: .string)
       }

       return copiedText?.isEmpty == false ? copiedText : nil
}
