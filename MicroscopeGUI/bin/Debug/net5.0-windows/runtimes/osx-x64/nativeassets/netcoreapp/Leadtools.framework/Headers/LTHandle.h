//
//  LTHandle.h
//  Leadtools
//
//  Copyright (c) 1991-2020 LEAD Technologies, Inc. All rights reserved.
//

NS_CLASS_AVAILABLE(10_10, 8_0)
@interface LTHandle : NSObject

- (nullable const void *)lock;
- (void)unlock;

@end
