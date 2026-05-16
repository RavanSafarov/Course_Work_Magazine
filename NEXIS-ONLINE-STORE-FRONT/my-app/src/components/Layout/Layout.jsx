import { Header } from './Header';

export const Layout = ({ children }) => (
    <div>
        <Header />
        <main className="wrap">{children}</main>
    </div>
);