class EditableText extends ZnodeBase {

    DialogDelete(DataTarget: string, target = undefined) {
        var selectedIds = DynamicGrid.prototype.GetMultipleSelectedIds(target);
        if (selectedIds.length > 0) {
            $('#' + DataTarget + '').modal('show');
        }
        else {
            $('#NoCheckboxSelected').modal('show');
        }
    }
}