using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace AppvNext.ObjectPicker
{
    public partial class ObjectPicker<TItem> : ComponentBase
    {
        [Parameter]
        public string ItemTypePlural { get; set; }

        [Parameter]
        public string TextPropertyName { get; set; }

        [Parameter]
        public string ValuePropertyName { get; set; }

        [Parameter]
        public List<TItem> AllItems { get; set; }

        [Parameter]
        public List<TItem> SelectedItems { get; set; }

        [Parameter]
        public EventCallback ComponentUpdated { get; set; }

        protected TItem SelectedItem { get; set; }
        protected List<TItem> AllItemsPrivate { get; set; }
        protected bool AddSelectedItemButtonDisabled = true;
        protected bool RemoveSelectedItemButtonDisabled = true;

        protected string ItemValue(TItem Item)
        {
            return Item.GetType()
            .GetProperty(ValuePropertyName)
            .GetValue(Item, null)
            .ToString();
        }

        protected string ItemText(TItem Item)
        {
            return Item.GetType()
            .GetProperty(TextPropertyName)
            .GetValue(Item, null)
            .ToString();
        }

        protected override void OnParametersSet()
        {
            // make a copy of the items
            AllItemsPrivate = AllItems.ToArray().ToList();

            if (AllItemsPrivate.Count > 0)
            {
                // remove the items that exist in SelectedItems
                foreach (var item in SelectedItems)
                {
                    var id = item.GetType()
                    .GetProperty(ValuePropertyName)
                    .GetValue(item, null)
                    .ToString();

                    var ItemFromAllItems =
                    (from x in AllItemsPrivate
                     where x.GetType()
                     .GetProperty(ValuePropertyName)
                     .GetValue(x, null)
                     .ToString() == id
                     select x).FirstOrDefault();

                    if (ItemFromAllItems != null)
                    {
                        AllItemsPrivate.Remove(ItemFromAllItems);
                    }
                }

            }
            if (AllItemsPrivate.Count > 0)
            {
                SelectedItem = AllItemsPrivate.First();
            }
            else if (SelectedItems.Count > 0)
            {
                SelectedItem = SelectedItems.First();
            }
            UpdateButtonEnabledStates();
        }

        protected void ItemSelectedFromAllItems(ChangeEventArgs args)
        {
            SelectedItem =
            (from x in AllItemsPrivate
             where x.GetType()
         .GetProperty(ValuePropertyName)
         .GetValue(x, null)
         .ToString() == args.Value.ToString()
             select x).FirstOrDefault();

            UpdateButtonEnabledStates();
        }

        protected void UpdateButtonEnabledStates()
        {
            AddSelectedItemButtonDisabled = !AllItemsPrivate.Contains(SelectedItem);
            RemoveSelectedItemButtonDisabled = !SelectedItems.Contains(SelectedItem);
        }

        protected void AddAllItems()
        {
            foreach (var Item in AllItemsPrivate.ToArray())
            {
                SelectedItems.Add(Item);
            }
            if (SelectedItems.Count > 0)
            {
                SelectedItem = SelectedItems.First();
            }
            AllItemsPrivate.Clear();
            UpdateButtonEnabledStates();
            ComponentUpdated.InvokeAsync().Wait();
        }

        protected void RemoveAllItems()
        {
            foreach (var Item in SelectedItems.ToArray())
            {
                AllItemsPrivate.Add(Item);
            }
            if (AllItemsPrivate.Count > 0)
            {
                SelectedItem = AllItemsPrivate.First();
            }
            SelectedItems.Clear();
            UpdateButtonEnabledStates();
            ComponentUpdated.InvokeAsync().Wait();
        }

        protected void AddSelectedItem()
        {
            if ((from x in SelectedItems
                 where ItemValue(x) == ItemValue(SelectedItem)
                 select x).FirstOrDefault() == null)
            {
                SelectedItems.Add(SelectedItem);
                AllItemsPrivate.Remove(SelectedItem);
                UpdateButtonEnabledStates();
                ComponentUpdated.InvokeAsync().Wait();
            }
        }

        protected void RemoveSelectedItem()
        {
            if ((from x in AllItemsPrivate
                 where ItemValue(x) == ItemValue(SelectedItem)
                 select x).FirstOrDefault() == null)
            {
                AllItemsPrivate.Add(SelectedItem);
                SelectedItems.Remove(SelectedItem);
                UpdateButtonEnabledStates();
                ComponentUpdated.InvokeAsync().Wait();
            }
        }

        protected void ItemSelectedFromSelectedItems(ChangeEventArgs args)
        {
            SelectedItem =
            (from x in SelectedItems
             where x.GetType()
             .GetProperty(ValuePropertyName)
             .GetValue(x, null)
             .ToString() == args.Value.ToString()
             select x
            ).FirstOrDefault();
            UpdateButtonEnabledStates();
        }

        protected void ItemDblClickedFromAllItems()
        {
            AddSelectedItem();
        }

        protected void ItemDblClickedFromSelectedItems()
        {
            RemoveSelectedItem();
        }

    }
}
