import React, { useState } from 'react';
import apiService from '../../services/apiService';

const AddGuestForm = () => {
    const [guest, setGuest] = useState({
      guestFirstname: '',
      guestLastname: '',
      guestPhoneNumber: '',
      guestEmail: '',
    });
  
    const handleChange = (e) => {
      setGuest({ ...guest, [e.target.name]: e.target.value });
    };
  
    const handleSubmit = (e) => {
      e.preventDefault();
      apiService.createGuest(guest)
        .then(() => {
          alert('Guest added successfully!');
          setGuest({ guestFirstname: '', guestLastname: '', guestPhoneNumber: '', guestEmail: '' });
        })
        .catch((err) => {
          console.error('Error adding guest:', err);
          alert('Failed to add guest');
        });
    };
  
    return (
      <form onSubmit={handleSubmit}>
        <h2>Add New Guest</h2>
        <input
          type="text"
          name="guestFirstname"
          placeholder="First Name"
          value={guest.guestFirstname}
          onChange={handleChange}
          required
        />
        <input
          type="text"
          name="guestLastname"
          placeholder="Last Name"
          value={guest.guestLastname}
          onChange={handleChange}
          required
        />
        <input
          type="text"
          name="guestPhoneNumber"
          placeholder="Phone Number"
          value={guest.guestPhoneNumber}
          onChange={handleChange}
          required
        />
        <input
          type="email"
          name="guestEmail"
          placeholder="Email"
          value={guest.guestEmail}
          onChange={handleChange}
          required
        />
        <button type="submit">Add Guest</button>
      </form>
    );
  };
  
  export default AddGuestForm;