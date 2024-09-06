import React, { useEffect } from 'react';

declare global {
    namespace JSX {
        interface IntrinsicElements {
            'my-couter': React.DetailedHTMLProps<React.HTMLAttributes<HTMLElement>, HTMLElement>;
        }
    }
}

declare global {
    interface Window {
        Blazor: {
            start: () => void;
            isStarted: boolean;
        }
    }
}

const loadBlazor = async () => {
    try {
        if (window.Blazor && !window.Blazor.isStarted) {
            await window.Blazor.start();
        }
    } catch (error) {
        console.error('Error starting Blazor:', error);
    }
};

const HeaderBlazor = () => {
    useEffect(() => {
        loadBlazor();
    }, []);

    return (
        <div>
            <h1>My React Component</h1>
            <my-couter increment-amount="10"></my-couter>
        </div>
    );
};

export default HeaderBlazor;