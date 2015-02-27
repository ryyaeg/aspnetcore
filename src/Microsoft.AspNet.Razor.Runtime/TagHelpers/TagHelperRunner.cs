﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Internal;
using Microsoft.Framework.WebEncoders;

namespace Microsoft.AspNet.Razor.Runtime.TagHelpers
{
    /// <summary>
    /// A class used to run <see cref="ITagHelper"/>s.
    /// </summary>
    public class TagHelperRunner
    {
        private readonly IHtmlEncoder _htmlEncoder;

        /// <summary>
        /// Instantiates a new instance of <see cref="TagHelperRunner"/>.
        /// </summary>
        /// <param name="htmlEncoder">The <see cref="IHtmlEncoder"/> used to encode HTML.</param>
        public TagHelperRunner([NotNull] IHtmlEncoder htmlEncoder)
        {
            _htmlEncoder = htmlEncoder;
        }

        /// <summary>
        /// Calls the <see cref="ITagHelper.ProcessAsync"/> method on <see cref="ITagHelper"/>s.
        /// </summary>
        /// <param name="executionContext">Contains information associated with running <see cref="ITagHelper"/>s.
        /// </param>
        /// <returns>Resulting <see cref="TagHelperOutput"/> from processing all of the
        /// <paramref name="executionContext"/>'s <see cref="ITagHelper"/>s.</returns>
        public async Task<TagHelperOutput> RunAsync([NotNull] TagHelperExecutionContext executionContext)
        {
            var tagHelperContext = new TagHelperContext(
                executionContext.AllAttributes,
                executionContext.Items,
                executionContext.UniqueId,
                executionContext.GetChildContentAsync);
            var tagHelperOutput = new TagHelperOutput(
                executionContext.TagName,
                executionContext.HTMLAttributes,
                _htmlEncoder)
            {
                SelfClosing = executionContext.SelfClosing,
            };
            var orderedTagHelpers = executionContext.TagHelpers.OrderBy(tagHelper => tagHelper.Order);

            foreach (var tagHelper in orderedTagHelpers)
            {
                await tagHelper.ProcessAsync(tagHelperContext, tagHelperOutput);
            }

            return tagHelperOutput;
        }
    }
}