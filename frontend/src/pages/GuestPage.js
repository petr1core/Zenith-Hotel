import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import apiService from "../services/apiService";
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  TextField,
} from "@mui/material";
import { v4 as uuidv4 } from "uuid";

const GuestPage = () => {
  const [guests, setGuests] = useState([]);
  const [openDialog, setOpenDialog] = useState(false);
  const [isEditMode, setIsEditMode] = useState(false);
  const [currentGuest, setCurrentGuest] = useState({
    guestId: null,
    guestFirstname: "",
    guestLastname: "",
    guestPhoneNumber: "",
    guestEmail: "",
  });

  useEffect(() => {
    fetchGuests();
  }, []);

  const fetchGuests = () => {
    apiService
      .getGuests()
      .then((response) => setGuests(response.data))
      .catch((error) => console.error("Error fetching guests:", error));
  };

  const handleInputChange = (e) => {
    setCurrentGuest({ ...currentGuest, [e.target.name]: e.target.value });
  };

  const handleAddGuest = () => {
    // Проверяем, что все поля заполнены
    if (
      !currentGuest.guestFirstname ||
      !currentGuest.guestLastname ||
      !currentGuest.guestPhoneNumber ||
      !currentGuest.guestEmail
    ) {
      alert("Please fill in all fields");
      return;
    }

    // Создаем нового гостя с данными из состояния currentGuest
    const newGuest = {
      guestId: uuidv4(), // Генерация UUID на клиенте
      guestFirstname: currentGuest.guestFirstname,
      guestLastname: currentGuest.guestLastname,
      guestPhoneNumber: currentGuest.guestPhoneNumber,
      guestEmail: currentGuest.guestEmail,
    };

    // Отправляем данные на сервер
    apiService
      .createGuest(newGuest)
      .then((response) => {
        console.log("Guest created:", response.data);
        setGuests([...guests, response.data]); // Добавляем нового гостя в список
        setCurrentGuest({
          guestId: null,
          guestFirstname: "",
          guestLastname: "",
          guestPhoneNumber: "",
          guestEmail: "",
        }); // Очищаем форму
        setOpenDialog(false); // Закрываем диалоговое окно
      })
      .catch((error) => {
        console.error("Error creating guest:", error);
      });
  };

  const handleEditGuest = (guest) => {
    setCurrentGuest(guest);
    setIsEditMode(true);
    setOpenDialog(true);
  };

  const handleUpdateGuest = () => {
    apiService
      .updateGuest(currentGuest.id, currentGuest)
      .then(() => {
        setGuests(
          guests.map((g) => (g.id === currentGuest.id ? currentGuest : g))
        );
        setCurrentGuest({
          id: null,
          guestFirstname: "",
          guestLastname: "",
          guestPhoneNumber: "",
          guestEmail: "",
        });
        setIsEditMode(false);
        setOpenDialog(false);
      })
      .catch((error) => console.error("Error updating guest:", error));
  };

  const handleDeleteGuest = (id) => {
    if (window.confirm("Are you sure you want to delete this guest?")) {
      apiService
        .deleteGuest(id)
        .then(() => setGuests(guests.filter((g) => g.id !== id)))
        .catch((error) => console.error("Error deleting guest:", error));
    }
  };

  return (
    <div>
      <h1>Manage Guests</h1>
      <Button
        variant="contained"
        color="primary"
        onClick={() => setOpenDialog(true)}
      >
        Add New Guest
      </Button>
      <TableContainer component={Paper} style={{ marginTop: 20 }}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>ID</TableCell>
              <TableCell>First Name</TableCell>
              <TableCell>Last Name</TableCell>
              <TableCell>Phone Number</TableCell>
              <TableCell>Email</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {guests.map((guest) => (
              <TableRow key={guest.id}>
                <TableCell>{guest.id}</TableCell>
                <TableCell>{guest.guestFirstname}</TableCell>
                <TableCell>{guest.guestLastname}</TableCell>
                <TableCell>{guest.guestPhoneNumber}</TableCell>
                <TableCell>{guest.guestEmail}</TableCell>
                <TableCell>
                  <Link to={`${guest.id}`}>
                    <Button size="small" color="primary">
                      View
                    </Button>
                  </Link>
                  <Button
                    size="small"
                    color="primary"
                    onClick={() => handleEditGuest(guest)}
                  >
                    Edit
                  </Button>
                  <Button
                    size="small"
                    color="secondary"
                    onClick={() => handleDeleteGuest(guest.id)}
                  >
                    Delete
                  </Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Диалоговое окно для добавления/редактирования гостя */}
      <Dialog open={openDialog} onClose={() => setOpenDialog(false)}>
        <DialogTitle>{isEditMode ? "Edit Guest" : "Add New Guest"}</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            name="guestFirstname"
            label="First Name"
            fullWidth
            value={currentGuest.guestFirstname}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="guestLastname"
            label="Last Name"
            fullWidth
            value={currentGuest.guestLastname}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="guestPhoneNumber"
            label="Phone Number"
            fullWidth
            value={currentGuest.guestPhoneNumber}
            onChange={handleInputChange}
          />
          <TextField
            margin="dense"
            name="guestEmail"
            label="Email Address"
            fullWidth
            value={currentGuest.guestEmail}
            onChange={handleInputChange}
          />
        </DialogContent>
        <DialogActions>
          <Button
            onClick={() => {
              setOpenDialog(false);
              setCurrentGuest({
                guestId: null,
                guestFirstname: "",
                guestLastname: "",
                guestPhoneNumber: "",
                guestEmail: "",
              });
              setIsEditMode(false);
            }}
            color="primary"
          >
            Cancel
          </Button>
          <Button
            onClick={isEditMode ? handleUpdateGuest : handleAddGuest}
            color="primary"
          >
            Save
          </Button>
        </DialogActions>
      </Dialog>
    </div>
  );
};

export default GuestPage;
