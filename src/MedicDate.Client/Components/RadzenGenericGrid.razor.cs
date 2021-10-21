using MedicDate.Client.Helpers;
using MedicDate.Shared.Models.Common;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using System.Linq.Dynamic.Core;

namespace MedicDate.Client.Components
{
    public partial class RadzenGenericGrid<TItem>
    {
        [Inject] public DialogService? DialogService { get; set; }

        [Parameter] public IEnumerable<TItem>? ItemList { get; set; }
        [Parameter] public int TotalCount { get; set; }

        [Parameter]
        public string[] Headers { get; set; } = Array.Empty<string>();

        [Parameter]
        public string[] PropNames { get; set; } = Array.Empty<string>();

        [Parameter] public RenderFragment? CustomGridCols { get; set; }

        [Parameter] public bool AllowFilter { get; set; } = true;
        [Parameter] public bool AllowColumnResize { get; set; } = true;

        [Parameter]
        public FilterMode FilterMode { get; set; } = FilterMode.Simple;

        [Parameter] public AllowCrudOps AllowCrudOps { get; set; } = new();

        [Parameter] public OpRoutes? OpRoutes { get; set; }

        [Parameter] public EventCallback<string> OnDeleteData { get; set; }

        private RadzenDataGrid<TItem>? _dataGrid;
        private IEnumerable<TItem>? _itemList;
        private bool _callLoadData;

        protected override void OnAfterRender(bool firstRender)
        {
            _callLoadData = !firstRender;
        }

        protected override void OnParametersSet()
        {
            if (ItemList is not null)
            {
                _itemList = ItemList;
            }
        }

        private async Task LoadData(LoadDataArgs? args = null, int pageIndex = 0
            , int pageSize = 10)
        {
            if (!_callLoadData)
            {
                return;
            }

            string url = "";

            if (OpRoutes is not null)
            {
                url = OpRoutes.GetUrl.Contains("?")
                    ? $"{OpRoutes.GetUrl}&pageIndex={pageIndex}&pageSize={pageSize}"
                    : $"{OpRoutes.GetUrl}?pageIndex={pageIndex}&pageSize={pageSize}";
            }

            var response =
                await _httpRepository.Get<PaginatedResourceListDto<TItem>>(url);

            if (!response.Error)
            {
                if (response.Response is not null)
                {
                    _itemList = response.Response.DataResult;
                    TotalCount = response.Response.TotalCount;
                }
            }

            if (args is not null)
            {
                var query = _itemList?.AsQueryable();
                if (!string.IsNullOrEmpty(args.Filter))
                {
                    query = query.Where(args.Filter);
                }

                if (!string.IsNullOrEmpty(args.OrderBy))
                {
                    query = query.OrderBy(args.OrderBy);
                }

                _itemList = query?.ToList();
            }
        }

        private async Task OpenDeleteWarningDialog(string? resourceId)
        {
            await DialogService!.OpenAsync<DeleteConfirmation>("Borrar Registro"
                , new Dictionary<string, object?>
                {
                    {
                        "Id"
                        , resourceId
                    }
                    , {"OnDelete", OnDeleteData}
                }, new DialogOptions { Width = "465px", Height = "280px" });
        }
    }
}