//
//  LTDicomLookupTableDescriptor.h
//  Leadtools.ImageProcessing.Core
//
//  Copyright (c) 1991-2020 LEAD Technologies, Inc. All rights reserved.
//

#import <Leadtools/LTRasterCommand.h>

NS_ASSUME_NONNULL_BEGIN

NS_CLASS_AVAILABLE(10_10, 8_0)
@interface LTDicomLookupTableDescriptor : NSObject

@property (nonatomic, assign) NSInteger firstStoredPixelValueMapped;
@property (nonatomic, assign) NSUInteger entryBits;

- (instancetype)initWithFirstStoredPixelValueMapped:(NSInteger)firstStoredPixelValueMapped entryBits:(NSUInteger)entryBits NS_DESIGNATED_INITIALIZER;

@end

NS_ASSUME_NONNULL_END
