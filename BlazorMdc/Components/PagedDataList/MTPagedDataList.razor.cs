﻿using BlazorMdc.Internal;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMdc
{
    /// <summary>
    /// A paged data list using the "wig pig" construct allowing the consumer to free render the relevant paged data.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public partial class MTPagedDataList<TItem> : ComponentFoundation
    {
        /// <summary>
        /// A CSS class to apply to the div surrounding the paged data.
        /// </summary>
        [Parameter] public string ListTemplateClass { get; set; } = "";


        /// <summary>
        /// A class for the paginator.
        /// </summary>
        [Parameter] public string PaginatorClass { get; set; } = "";


        /// <summary>
        /// A list of allowable numbers of items per page for the paginator.
        /// </summary>
        [Parameter] public IEnumerable<int> ItemsPerPageSelection { get; set; }


        /// <summary>
        /// A function delegate to return the parameters for <c>@key</c> attributes. If unused
        /// "fake" keys set to GUIDs will be used.
        /// </summary>
        [Parameter] public Func<TItem, object> GetKeysFunc { get; set; }


        /// <summary>
        /// The pageable data.
        /// </summary>
        [Parameter] public IEnumerable<TItem> Data { get; set; }


        /// <summary>
        /// The wig pig item renderfragment.
        /// </summary>
        [Parameter] public RenderFragment<TItem> ItemTemplate { get; set; }


        /// <summary>
        /// The wig pig list renderfragment.
        /// </summary>
        [Parameter] public RenderFragment<RenderFragment> ListTemplate { get; set; }


        private int BackingPageNumber
        {
            get => PageNumber;
            set
            {
                if (value != PageNumber)
                {
                    var oldValue = PageNumber;
                    PageNumber = value;

                    if (HasRendered)
                    {
                        PageNumberChanged.InvokeAsync(value);
                        InvokeAsync(() => OnPageNumberChange(oldValue, value));
                    }
                }
            }
        }

        /// <summary>
        /// The page number.
        /// </summary>
        [Parameter] public int PageNumber { get; set; }


        /// <summary>
        /// Change handler for <see cref="PageNumber"/>.
        /// </summary>
        [Parameter] public EventCallback<int> PageNumberChanged { get; set; }


        private int BackingItemsPerPage
        {
            get => ItemsPerPage;
            set
            {
                if (value != ItemsPerPage)
                {
                    ItemsPerPage = value;
                    if (HasRendered) ItemsPerPageChanged.InvokeAsync(value);
                }
            }
        }

        /// <summary>
        /// The number of items per page.
        /// </summary>
        [Parameter] public int ItemsPerPage { get; set; }

        /// <summary>
        /// Change handler for <see cref="ItemsPerPage"/>.
        /// </summary>
        [Parameter] public EventCallback<int> ItemsPerPageChanged { get; set; }


        private IEnumerable<TItem> CheckedData => Data ?? Array.Empty<TItem>();
        private string ContentClass { get; set; } = "";
        public IEnumerable<TItem> CurrentPage => CheckedData.Skip(PageNumber * ItemsPerPage).Take(ItemsPerPage);
        private bool HasRendered { get; set; } = false;
        private bool IsHidden { get; set; } = false;
        private Func<TItem, object> KeyGenerator { get; set; }


        private async Task OnPageNumberChange(int oldPageNumber, int newPageNumber)
        {
            string nextClass;
            if (newPageNumber < oldPageNumber)
            {
                nextClass = MTSlidingContent<object>.InFromLeft;
                ContentClass = MTSlidingContent<object>.OutToRight;
            }
            else
            {
                nextClass = MTSlidingContent<object>.InFromRight;
                ContentClass = MTSlidingContent<object>.OutToLeft;
            }

            await Task.Delay(100);

            ContentClass = nextClass;
            IsHidden = false;

            StateHasChanged();
        }


        // Would like to use <inheritdoc/> however DocFX cannot resolve to references outside BlazorMdc
        protected override void OnInitialized()
        {
            base.OnInitialized();

            ClassMapper
                .AddIf(MTSlidingContent<object>.Hidden, () => IsHidden)
                .AddIf(MTSlidingContent<object>.Visible, () => !IsHidden);
        }


        // Would like to use <inheritdoc/> however DocFX cannot resolve to references outside BlazorMdc
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            HasRendered = true;
        }
    }
}
