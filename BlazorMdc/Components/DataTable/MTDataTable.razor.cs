﻿using BlazorMdc.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorMdc
{
    /// <summary>
    /// This is a general purpose Material Theme data table.
    /// </summary>
    public partial class MTDataTable<TItem> : ComponentFoundation
    {
        /// <summary>
        /// A function delegate to return the parameters for <c>@key</c> attributes. If unused
        /// "fake" keys set to GUIDs will be used.
        /// </summary>
        [Parameter] public Func<TItem, object> GetKeysFunc { get; set; }


        /// <summary>
        /// Data to render in the <see cref="TableRow"/> render fragment.
        /// </summary>
        [Parameter] public IEnumerable<TItem> Items { get; set; }


        /// <summary>
        /// The table header as a render fragment using <c>&lt;th&gt;</c> HTML elements.
        /// </summary>
        [Parameter] public RenderFragment TableHeader { get; set; }


        /// <summary>
        /// Render fragment for each row of the table using <c>&lt;td&gt;</c> HTML elements.
        /// </summary>
        [Parameter] public RenderFragment<TItem> TableRow { get; set; }


        /// <summary>
        /// The data table's density.
        /// </summary>
        [Parameter] public MTDensity? Density { get; set; }


        private MTCascadingDefaults.DensityInfo DensityInfo => CascadingDefaults.GetDensityCssClass(CascadingDefaults.AppliedDataTableDensity(Density));
        private ElementReference ElementReference { get; set; }
        private Func<TItem, object> KeyGenerator { get; set; }


        // Would like to use <inheritdoc/> however DocFX cannot resolve to references outside BlazorMdc
        protected override void OnInitialized()
        {
            base.OnInitialized();

            ClassMapper
                .Add("mdc-data-table")
                .AddIf(DensityInfo.CssClassName, () => DensityInfo.ApplyCssClass);
        }


        // Would like to use <inheritdoc/> however DocFX cannot resolve to references outside BlazorMdc
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            KeyGenerator = GetKeysFunc ?? delegate (TItem item) { return item; };
        }


        /// <inheritdoc/>
        private protected async override Task InitializeMdcComponent() => await JsRuntime.InvokeVoidAsync("BlazorMdc.dataTable.init", ElementReference);
    }
}
