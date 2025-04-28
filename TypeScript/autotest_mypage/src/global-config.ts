export interface DataLayerEvent {
    event: string;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    [key: string]: any;
}

declare global {
    interface Window {
        __INITIAL_STATE__: { cms: { pageContent: { MetaInfo: { Title: string } } } };
        dataLayer: DataLayerEvent[];
    }
}

export { };
