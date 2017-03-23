﻿// <copyright file="IccRenderingIntent.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp
{
    /// <summary>
    /// Rendering intent
    /// </summary>
    internal enum IccRenderingIntent : uint
    {
        /// <summary>
        /// In perceptual transforms the PCS values represent hypothetical
        /// measurements of a colour reproduction on the reference reflective
        /// medium. By extension, for the perceptual intent, the PCS represents
        /// the appearance of that reproduction as viewed in the reference viewing
        /// environment by a human observer adapted to that environment. The exact
        /// colour rendering of the perceptual intent is vendor specific.
        /// </summary>
        Perceptual = 0,

        /// <summary>
        /// Transformations for this intent shall re-scale the in-gamut,
        /// chromatically adapted tristimulus values such that the white
        /// point of the actual medium is mapped to the PCS white point
        /// (for either input or output)
        /// </summary>
        MediaRelativeColorimetric = 1,

        /// <summary>
        /// The exact colour rendering of the saturation intent is vendor
        /// specific and involves compromises such as trading off
        /// preservation of hue in order to preserve the vividness of pure colours.
        /// </summary>
        Saturation = 2,

        /// <summary>
        /// Transformations for this intent shall leave the chromatically
        /// adapted nCIEXYZ tristimulus values of the in-gamut colours unchanged.
        /// </summary>
        AbsoluteColorimetric = 3,
    }
}
