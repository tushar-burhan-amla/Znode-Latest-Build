class PostalCodeRegExp extends ZnodeBase {
    constructor() {
        super();
    }
}

module PostalCodeValidationRegExp {
    export const US = /^[0-9]{5}(?:-[0-9]{4})?$/;
    export const IN = /^[1-9][0-9]{5}$/;
}