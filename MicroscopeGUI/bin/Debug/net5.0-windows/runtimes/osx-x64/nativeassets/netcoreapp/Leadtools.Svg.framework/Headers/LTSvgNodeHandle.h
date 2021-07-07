//
//  LTSvgNodeHandle.h
//  Leadtools.Svg
//
//  Copyright (c) 1991-2020 LEAD Technologies, Inc. All rights reserved.
//

#import <Leadtools/LTPrimitives.h>
#import <Leadtools.Svg/LTEnums.h>

NS_ASSUME_NONNULL_BEGIN

NS_CLASS_AVAILABLE(10_10, 8_0)
@interface LTSvgNodeHandle : NSObject

@property (nonatomic, assign)                           LTSvgElementType elementType;

@property (nonatomic, assign, readonly)                 LeadRectD bounds;

- (instancetype)init; //This should not be called directly

- (nullable NSString *)elementName:(NSError **)error;

- (nullable NSString *)elementValue:(NSError **)error;
- (BOOL)setElementValue:(NSString *)value;

- (nullable NSString *)valueOfAttribute:(NSString *)attribute error:(NSError **)error;
- (BOOL)setValue:(NSString *)value forAttribute:(NSString *)attribute error:(NSError **)error;
- (BOOL)removeElementAttribute:(NSString *)attribute error:(NSError **)error;

@end

NS_ASSUME_NONNULL_END
