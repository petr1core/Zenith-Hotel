import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import apiService from "../services/apiService";

const GuestDetailPage = () => {
    const { id } = useParams(); // Получаем id из URL
    const [guest, setGuest] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        apiService.getGuestById(id) // Загружаем данные о госте
            .then((response) => {
                setGuest(response.data);
                setLoading(false);
            })
            .catch((err) => {
                setError("Failed to load guest details");
                setLoading(false);
            });
    }, [id]);

    if (loading) return <p>Loading...</p>;
    if (error) return <p>{error}</p>;
    if (!guest) return <p>Guest not found</p>;

    return (
        <div style={{ padding: "20px" }}>
            <h1>Guest Details</h1>
            <table border="1" style={{ width: "100%", borderCollapse: "collapse", marginTop: "20px" }}>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>First Name</th>
                        <th>Last Name</th>
                        <th>Phone Number</th>
                        <th>Email</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>{guest.guestId}</td>
                        <td>{guest.guestFirstname}</td>
                        <td>{guest.guestLastname}</td>
                        <td>{guest.guestPhoneNumber}</td>
                        <td>{guest.guestEmail}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    );
};

export default GuestDetailPage;